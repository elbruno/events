# Bruno Capuano
# open camera with openCV
# analyze camera frame with local docker custom vision project
# draw bounding boxes for each reconized object
# key D enable / disable instance segmentation detection

import time
import cv2
import urllib
import json
import requests
import os
import asyncio
import datetime

from flask import Flask, request, jsonify
from parkinglot_device import ParkingLot_Device


# -----------------------------------------------
# Local calls
# -----------------------------------------------

probabilityThreshold = 25

def displayPredictions(jsonPrediction, img, frameImageFileName, jsonStr):
    global camera_Width, camera_Heigth
    global pcdevice, slot1, slot2, slot3, slot4

    jsonObj = json.loads(jsonPrediction)
    preds = jsonObj['predictions']
    sorted_preds = sorted(preds, key=lambda x: x['probability'], reverse=True)
    strSortedPreds = ""
    resultFound = False
    slot1 = 0
    slot2 = 0
    slot3 = 0
    slot4 = 0
    if (sorted_preds):
        detected = False
        for pred in sorted_preds:
            # tag name and prob * 100
            tagName     = str(pred['tagName'])
            probability = pred['probability'] * 100
            # apply threshold
            if (probability >= probabilityThreshold):
                detected = True
                bb = pred['boundingBox']

                # adjust to size
                height = int(bb['height'] * camera_Heigth)
                left = int(bb['left'] * camera_Width)
                top = int(bb['top'] * camera_Heigth)
                width = int(bb['width'] * camera_Width)

                # draw bounding boxes
                start_point = (left, top)                 
                end_point = (left + width, top + height) 
                color = (255, 0, 0) 
                thickness = 2                
                cv2.rectangle(img, start_point, end_point, color, thickness)                 
                print(jsonPrediction)

                if(tagName == "spot1"):
                    slot1 = 1
                elif (tagName == "spot2"):
                    slot2 = 1
                elif (tagName == "spot3"):
                    slot3 = 1
                elif (tagName == "spot4"):
                    slot4 = 1                                        

        if (detected == True):
            detImageFileName = frameImageFileName.replace('tmp', 'det')
            cv2.imwrite(detImageFileName, img)                
            detJsonFileName = detImageFileName.replace('png', 'json')
            save_text = open(detJsonFileName, 'w')
            save_text.write(jsonStr)
            save_text.close()              

                
    return strSortedPreds

# -----------------------------------------------
# AZURE IOT CENTRAL
# -----------------------------------------------

async def init_AzureIoT():
    global pcdevice

    iothub    = ""
    scope     = ""
    device_id = ""
    key       = ""
    pcdevice  = ParkingLot_Device(scope, device_id, key)
    await pcdevice.init_azureIoT()

# -----------------------------------------------
# Main program
# -----------------------------------------------
async def main():
    global pcdevice, slot1, slot2, slot3, slot4
    global camera_Width, camera_Heigth
    global img

    # instantiate flask app and push a context
    app = Flask(__name__)

    await init_AzureIoT()

    # open UDP
    camera_Width = 640 #1280
    camera_Heigth = 480 #960

    # open UDP
    cap = cv2.VideoCapture(1)

    dsize = (camera_Width, camera_Heigth)

    # open
    detectionEnabled = False
    i = 0
    while True:
        i = i + 1
        imgNumber = str(i).zfill(5)
        start_time = time.time()

        #try:
        ret, frame = cap.read()
        img = cv2.resize(frame, (camera_Width, camera_Heigth))

        if  (detectionEnabled):
            # save image to disk and open it
            frameImageFileName = str(f'tmp\image{imgNumber}.png')
            cv2.imwrite(frameImageFileName, img)

            with open(frameImageFileName, 'rb') as f:
                img_data = f.read()

            # analyze file in local container
            api_url = "http://127.0.0.1:80/image"
            r = requests.post(api_url, data=img_data)
            with app.app_context():
                jsonResults = jsonify(r.json())
            jsonStr = jsonResults.get_data(as_text=True)
            displayPredictions(jsonStr, img, frameImageFileName, jsonStr)

            if (i % 10) == 0:
                await pcdevice.send_telemetry(slot1, slot2, slot3, slot4)
                await pcdevice.send_properties(slot1, slot2, slot3, slot4)


        fpsInfo = ""
        if (time.time() - start_time ) > 0:
            fpsInfo = "FPS: " + str(1.0 / (time.time() - start_time)) # FPS = 1 / time to process loop
            font = cv2.FONT_HERSHEY_DUPLEX
            cv2.putText(img, fpsInfo, (10, 20), font, 0.4, (255, 255, 255), 1)

        cv2.imshow('@elbruno - Camera', img)
        # except Exception as e:
        #     detectionEnabled = False
        #     print(f'exc: {e}')
        #     pass

        # key controller
        key = cv2.waitKey(1) & 0xFF    
        if key == ord("d"):
            if (detectionEnabled == True):
                detectionEnabled = False
            else:
                detectionEnabled = True

        if key == ord("q"):
            break
        

    # close the already opened camera, and the video file
    cap.release()
    cv2.destroyAllWindows()

if __name__ == "__main__":
    asyncio.run(main())
