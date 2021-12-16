/*
   Copyright (c) 2021
   Author      : Bruno Capuano
   Create Time : 2021 December
   Change Log  :

   - Demo for 
   - UltraSonic Ranger to close movement. Sensor https://www.seeedstudio.com/Grove-Ultrasonic-Distance-Sensor.html
   - UltraSonic Ranger connected on left grove port

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
#include "demoicons.h"
#include "Ultrasonic.h"
#include "TFT_eSPI.h"

TFT_eSPI tft;
Ultrasonic ultrasonic(PIN_WIRE_SCL); // sensor connected on left grove port

void setup()
{
  Serial.begin(9600);

  // TFT Init
  tft.begin();
  tft.setRotation(3);
  tft.fillScreen(TFT_BLACK);
  tft.setTextSize(4);
}

void loop()
{
  long RangeInCentimeters;
  RangeInCentimeters = ultrasonic.MeasureInCentimeters();
  Serial.println(RangeInCentimeters);

  tft.setCursor(20, 20);
  tft.print("                         ");
  tft.setCursor(20, 20);
  tft.print(String(RangeInCentimeters));

  if (RangeInCentimeters < 5)
    tft.drawXBitmap(left_pos_x, left_pos_y, unchecked_bits, xbm_width, xbm_height, TFT_RED, TFT_BLACK);
  else
    tft.drawXBitmap(left_pos_x, left_pos_y, checked_bits, xbm_width, xbm_height, TFT_GREEN, TFT_BLACK);

  delay(1000);
}