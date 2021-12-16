/*
   Copyright (c) 2021
   Author      : Bruno Capuano
   Create Time : 2021 December
   Change Log  :

   - Demo buttons and TFT interaction

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
#include "TFT_eSPI.h"
TFT_eSPI tft;

void setup()
{
  Serial.begin(9600);

  // init buttons
  Serial.println("Init buttons");
  pinMode(WIO_KEY_A, INPUT_PULLUP);
  pinMode(WIO_KEY_B, INPUT_PULLUP);
  pinMode(WIO_KEY_C, INPUT_PULLUP);

  delay(1000);

  // TFT Init
  tft.begin();
  tft.setRotation(3);
  tft.fillScreen(TFT_BLACK);
  tft.setTextSize(4);
}

void loop()
{
  bool buttonPressed = false;
  String message = "";
  tft.setCursor(20, 20);
  tft.print("                         ");

  // validate buttons
  if (digitalRead(WIO_KEY_A) == LOW)
  {
    message = "BUTTON [A]";
    buttonPressed = true;
  }
  if (digitalRead(WIO_KEY_B) == LOW)
  {
    message = "BUTTON [B]";
    buttonPressed = true;
  }
  if (digitalRead(WIO_KEY_C) == LOW)
  {
    message = "BUTTON [C]";
    buttonPressed = true;
  }

  tft.setCursor(20, 20);
  tft.print(message);

  if (buttonPressed)
    tft.drawXBitmap(left_pos_x, left_pos_y, checked_bits, xbm_width, xbm_height, TFT_GREEN, TFT_BLACK);
  else
    tft.drawXBitmap(left_pos_x, left_pos_y, unchecked_bits, xbm_width, xbm_height, TFT_RED, TFT_BLACK);

  Serial.println(message);

  delay(1000);
}