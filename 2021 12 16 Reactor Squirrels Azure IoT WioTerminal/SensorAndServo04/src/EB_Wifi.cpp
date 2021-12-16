/*
   Copyright (c) 2021
   Author      : Bruno Capuano
   Create Time : 2021 December
   Change Log  :

   - Additional functions to work with Wifi and IP Addresses

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

#include <EB_Wifi.h>

// ==============================
// WIFI
// ==============================

String ip2Str(IPAddress ip)
{
  String s = "";
  for (int i = 0; i < 4; i++)
  {
    s += i ? "." + String(ip[i]) : String(ip[i]);
  }
  return s;
}

String wifiStatus2Str(int wifi_status)
{
  // typedef enum {
  //     WL_NO_SHIELD        = 255,   // for compatibility with WiFi Shield library
  //     WL_IDLE_STATUS      = 0,
  //     WL_NO_SSID_AVAIL    = 1,
  //     WL_SCAN_COMPLETED   = 2,
  //     WL_CONNECTED        = 3,
  //     WL_CONNECT_FAILED   = 4,
  //     WL_CONNECTION_LOST  = 5,
  //     WL_DISCONNECTED     = 6
  // } wl_status_t;
  String result = "NO_SHIELD";
  if (wifi_status == 0)
    result = "IDLE_STATUS";
  else if (wifi_status == 1)
    result = "NO_SSID_AVAIL";
  else if (wifi_status == 2)
    result = "SCAN_COMPLETED";    
  else if (wifi_status == 3)
    result = "CONNECTED";
  else if (wifi_status == 4)
    result = "CONNECT_FAILED";
  else if (wifi_status == 5)
    result = "CONNECTION_LOST";    
  else if (wifi_status == 6)
    result = "DISCONNECTED";    

  return result;
}