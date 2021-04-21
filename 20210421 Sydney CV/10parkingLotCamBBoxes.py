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
from flask import Flask, request, jsonify



# -----------------------------------------------
# Local calls
# -----------------------------------------------

probabilityThreshold = 25

def displayPredictions(jsonPrediction, frame):
    global camera_Width, camera_Heigth
    jsonObj = json.loads(jsonPrediction)
    preds = jsonObj['predictions']
    sorted_preds = sorted(preds, key=lambda x: x['probability'], reverse=True)
    strSortedPreds = ""
    resultFound = False
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

        if (detected == True):
            detImageFileName = frameImageFileName.replace('tmp', 'det')
            cv2.imwrite(detImageFileName, img)                
            detJsonFileName = detImageFileName.replace('png', 'json')
            save_text = open(detJsonFileName, 'w')
            save_text.write(jsonStr)
            save_text.close()              

                
    return strSortedPreds

# instantiate flask app and push a context
app = Flask(__name__)

# -----------------------------------------------
# Main program
# -----------------------------------------------

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

    try:
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
            displayPredictions(jsonStr, frame)

        # overlay background
        # img = cv2.addWeighted(background, 1, img, 1, 0)

        fpsInfo = ""
        if (time.time() - start_time ) > 0:
            fpsInfo = "FPS: " + str(1.0 / (time.time() - start_time)) # FPS = 1 / time to process loop
            font = cv2.FONT_HERSHEY_DUPLEX
            cv2.putText(img, fpsInfo, (10, 20), font, 0.4, (255, 255, 255), 1)

        cv2.imshow('@elbruno - Camera', img)
    except Exception as e:
        detectionEnabled = False
        print(f'exc: {e}')
        pass

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