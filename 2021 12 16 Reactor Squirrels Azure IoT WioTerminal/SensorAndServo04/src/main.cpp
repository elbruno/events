/*
   Copyright (c) 2021
   Author      : Bruno Capuano
   Create Time : 2021 December
   Change Log  :

   - Custom implementation for Wio Terminal of the Arduino Servo Library https://www.arduino.cc/reference/en/libraries/servo/
   - UltraSonic Ranger to close movement. Sensor https://www.seeedstudio.com/Grove-Ultrasonic-Distance-Sensor.html
   - UltraSonic Ranger connected on left grove port
   - Wio Terminal screen display a Work in progress animated clock
   - Close object detected
      - show a squirrel for 5 seconds and open the servo
      - after 5 seconds, hide the squirrel and close the servo
   - Auto Connect to Wifi. Try to reconnect if disconnected.
   - Once connected to Wifi, get time from NTP server
   - When feeder state change, send telemetry to Azure IoT. Telemetry includes feeder state and range sensor
   - Servo aligned to fit the ope/close lid for the Squirrel feeder
   - Display log on small text on the bottom of the device with current range sensor, wifi state and telemetry sent
   - Process invoked methods from Azure IoT Hub. On [squirrelDetected] it will for a Squirrel Detected event

   The MIT License (MIT)

   Permission is hereby granted, free of charge, to any person obtaining a copy
   of this software and associated documentation files (the "Software"), to deal
   in the Software without restriction, including without limitation the rights
   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
   copies of the Software, and to permit persons to whom the Software is
   furnished to do so, subject to the following conditions:

   The above copyright notice and this permission notice shall be included in
   all copies or substantial portions of the Software.

   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
   THE SOFTWARE.
*/

#include <Arduino.h>
#include <ArduinoJson.h>
#include <rpcWiFi.h>
#include <SPI.h>
#include "config.h"

// custom includes for Servo, Ultrasonic sensor and TFT work
#include "EB_Wifi.h"
#include "EB_TFT.h"
#include "EB_Servo.h"
#include "Ultrasonic.h"
#include "wipsquirrelicons.h" // Sketch tab header for xbm images
//#include "EB_AzureIoTHub.h"

// Azure IoT
#include <AzureIoTHub.h>
#include <AzureIoTProtocol_MQTT.h>
#include <iothubtransportmqtt.h>
#include "ntp.h"

// debug
bool debugTftMsg = true;

// servo
Servo myservo;
const long detectedSquirrelDoorOpenTime = 5000;
unsigned long lastTimeMoveServo = 0;
long servoPos = 0;
const long servoOpen = 50;
const long servoClosed = 90;
bool currentStateOpen = false;
bool newStateOpen = false;

// Ultrasonic
Ultrasonic ultrasonic(PIN_WIRE_SCL); // sensor connected on left grove port

// Counters / validators
int wipCounter = 0;
int minDistanceToDetect = 10;
bool squirrelDetected = false;
unsigned long squirrelDetectedTime = 0;

///////////////////////////////
// Azure IoT
///////////////////////////////

IOTHUB_DEVICE_CLIENT_LL_HANDLE _device_ll_handle;
int source_invoked_counter = 0;

static void connectionStatusCallback(IOTHUB_CLIENT_CONNECTION_STATUS result, IOTHUB_CLIENT_CONNECTION_STATUS_REASON reason, void *user_context)
{
  if (result == IOTHUB_CLIENT_CONNECTION_AUTHENTICATED)
  {
    Serial.println("The device client is connected to iothub");
  }
  else
  {
    Serial.println("The device client has been disconnected");
  }
}

int directMethodCallback(const char *method_name, const unsigned char *payload, size_t size, unsigned char **response, size_t *response_size, void *userContextCallback)
{
  Serial.printf("Direct method received %s\r\n", method_name);

  if (strcmp(method_name, "squirrelDetected") == 0)
  {
    squirrelDetected = true;
    squirrelDetectedTime = millis();
    source_invoked_counter = 5;
  }

  char resultBuff[16];
  sprintf(resultBuff, "{\"Result\":\"\"}");
  *response_size = strlen(resultBuff);
  *response = (unsigned char *)malloc(*response_size);
  memcpy(*response, resultBuff, *response_size);

  return IOTHUB_CLIENT_OK;
}

void connectIoTHub()
{
  IoTHub_Init();

  _device_ll_handle = IoTHubDeviceClient_LL_CreateFromConnectionString(CONNECTION_STRING, MQTT_Protocol);

  if (_device_ll_handle == NULL)
  {
    Serial.println("Failure creating Iothub device. Hint: Check your connection string.");
    return;
  }

  IoTHubDeviceClient_LL_SetConnectionStatusCallback(_device_ll_handle, connectionStatusCallback, NULL);
  IoTHubClient_LL_SetDeviceMethodCallback(_device_ll_handle, directMethodCallback, NULL);
}

void sendTelemetry(const char *telemetry)
{
  IOTHUB_MESSAGE_HANDLE message_handle = IoTHubMessage_CreateFromString(telemetry);
  IoTHubDeviceClient_LL_SendEventAsync(_device_ll_handle, message_handle, NULL, NULL);
  IoTHubMessage_Destroy(message_handle);
}

void work_delay(int delay_time)
{
  int current = 0;
  do
  {
    IoTHubDeviceClient_LL_DoWork(_device_ll_handle);
    delay(100);
    current += 100;
  } while (current < delay_time);
}

///////////////////////////////
// TFT Icons
///////////////////////////////

void displayWorkInProgressWheel()
{
  if (wipCounter == 1)
  {
    tft.drawXBitmap(right_pos_x, right_pos_y, xbmWip04_bits, xbm_width, xbm_height, TFT_WHITE, TFT_BLACK);
  }
  else if (wipCounter == 2)
  {
    tft.drawXBitmap(right_pos_x, right_pos_y, xbmWip03_bits, xbm_width, xbm_height, TFT_WHITE, TFT_BLACK);
  }
  else if (wipCounter == 3)
  {
    tft.drawXBitmap(right_pos_x, right_pos_y, xbmWip02_bits, xbm_width, xbm_height, TFT_WHITE, TFT_BLACK);
  }
  else
  {
    wipCounter = 0;
    tft.drawXBitmap(right_pos_x, right_pos_y, xbmWip01_bits, xbm_width, xbm_height, TFT_WHITE, TFT_BLACK);
  }
  wipCounter++;
}

///////////////////////////////
// Ultrasonic Range
///////////////////////////////

long getUltrasonicRange()
{
  long RangeInCentimeters = ultrasonic.MeasureInCentimeters();
  Serial.print("Ultra Sonic: ");
  Serial.println(RangeInCentimeters);
  if (debugTftMsg)
    tftPrintLog(String(RangeInCentimeters), tft_col_log0);
  return RangeInCentimeters;
}

long ValidateUltraSonicRangeValue()
{
  long range_detected = getUltrasonicRange();
  if (range_detected < minDistanceToDetect)
  {
    squirrelDetected = true;
    squirrelDetectedTime = millis();
  }
  else
  {
    squirrelDetected = false;
  }
  return range_detected;
}

// ==============================
// WIFI
// ==============================

void connectWiFi()
{
  tftInit();

  int startWifiTime = millis();
  int connectAttemps = 0;
  Serial.print("Connecting to Wifi ...");
  tftPrintLine(tft_row3, "Connecting to Wifi ...", "");
  tftPrintLine(tft_row4, "SSID:", SSID);
  Serial.println("before mode");
  WiFi.mode(WIFI_STA);
  Serial.println("before disconnect");
  WiFi.disconnect();
  Serial.println("after disconnect");

  while (WiFi.status() != WL_CONNECTED)
  {
    int secondsWaiting = (millis() - startWifiTime) / 1000;
    tftPrintLine(tft_row5, "Seconds waiting: ", String(secondsWaiting));
    tftPrintLine(tft_row6, "Connection attemps: ", String(connectAttemps));
    Serial.println("Connecting to WiFi..");
    Serial.print("Seconds waiting : ");
    Serial.println(secondsWaiting);
    WiFi.begin(SSID, PASSWORD);
    delay(500);
    connectAttemps++;
  }

  // Clean screen to show results
  tftInit();

  Serial.println("Connected!");
  Serial.println(WiFi.localIP());
  tftPrintLine(tft_row5, "Connected to ", SSID);
  tftPrintLine(tft_row6, "IP: ", ip2Str(WiFi.localIP()));

  delay(2000);

  tftInit();
}

///////////////////////////////
// Online Services
///////////////////////////////
void validateOnlineConnection()
{
  if (debugTftMsg)
    tftPrintLog(wifiStatus2Str(WiFi.status()), tft_col_log1);

  if (WiFi.status() != WL_CONNECTED)
  {
    connectWiFi();
    initTime();
    connectIoTHub();
  }
}

void validateButtonsState()
{
  // validate buttons state
  if (digitalRead(WIO_KEY_A) == LOW)
  {
    Serial.println("[A] OPEN");
    squirrelDetected = true;
    squirrelDetectedTime = millis();
    source_invoked_counter = 5;
  }
  if (digitalRead(WIO_KEY_B) == LOW)
  {
    Serial.println("[B] CLOSED ");
    squirrelDetected = false;
  }
  if (digitalRead(WIO_KEY_C) == LOW)
  {
    Serial.println("[C] DEBUG ");
    Serial.println(debugTftMsg);
    debugTftMsg = !debugTftMsg;
  }
}

///////////////////////////////
// Main App
///////////////////////////////
void setup()
{
  Serial.begin(9600);

  // connect to online services
  connectWiFi();
  initTime();
  connectIoTHub();

  // Init servo
  myservo.attach(D0); // working on right grove port
  myservo.write(servoClosed);

  // init buttons
  Serial.println("Init buttons");
  pinMode(WIO_KEY_A, INPUT_PULLUP);
  pinMode(WIO_KEY_B, INPUT_PULLUP);
  pinMode(WIO_KEY_C, INPUT_PULLUP);

  delay(1000);
}

void loop()
{
  // in progress
  displayWorkInProgressWheel();
  validateOnlineConnection();

  // check sensors and actuators only when not Azure IoT method invoke is present
  long range_detected = 0;
  if (source_invoked_counter > 0)
    source_invoked_counter--;
  else
  {
    validateButtonsState();
    range_detected = ValidateUltraSonicRangeValue();
  }

  // squirrel detected logic
  if (squirrelDetected == true)
  {
    newStateOpen = true;
    tftPrintLine(tft_row3, "  Squirrel Detected !", "");
    tft.drawXBitmap(left_pos_x, left_pos_y, xbmSquirrel_bits, xbm_width, xbm_height, TFT_RED, TFT_BLACK);
  }
  else if ((millis() - squirrelDetectedTime) > detectedSquirrelDoorOpenTime)
  {
    newStateOpen = false;
    tftPrintLine(tft_row3, "", "");
    tft.drawXBitmap(left_pos_x, left_pos_y, xbmTree_bits, xbm_width, xbm_height, TFT_GREEN, TFT_BLACK);
    tftPrintLog("          ", tft_col_log2);
  }

  // validate if current state is different than new state
  // only move servo after N seconds from last servo move
  if ((newStateOpen != currentStateOpen) && ((millis() - lastTimeMoveServo) > detectedSquirrelDoorOpenTime))
  {
    // update current state
    currentStateOpen = newStateOpen;
    Serial.print("Current State Open: ");
    Serial.println(currentStateOpen);

    // define position to move
    if (currentStateOpen)
      servoPos = servoOpen;
    else
      servoPos = servoClosed;

    Serial.print("Servo Pos: ");
    Serial.println(servoPos);
    myservo.write(servoPos);
    lastTimeMoveServo = millis();

    // send telemetry to Azure IoT
    DynamicJsonDocument doc(1024);
    doc["feeder_state"] = squirrelDetected;
    doc["range_detected"] = range_detected;

    string telemetry;
    serializeJson(doc, telemetry);

    Serial.print("Sending telemetry ");
    Serial.println(telemetry.c_str());
    sendTelemetry(telemetry.c_str());

    source_invoked_counter = 0;

    if (debugTftMsg)
      tftPrintLog("telemetry", tft_col_log2);
  }

  work_delay(200);
}