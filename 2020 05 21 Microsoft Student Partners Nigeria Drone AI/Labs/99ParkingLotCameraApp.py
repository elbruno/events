import os
import cv2
import time
import urllib
import json
import requests

from flask import Flask, request, jsonify

probabilityThreshold = 15

def getPredictionsSorted(jsonPrediction):
  jsonObj = json.loads(jsonPrediction)
  preds = jsonObj['predictions']
  sorted_preds = sorted(preds, key=lambda x: x['probability'], reverse=True)
  strSortedPreds = ""
  if (sorted_preds):
    for pred in sorted_preds:
        # tag name and prob * 100
        tagName     = str(pred['tagName'])
        probability = pred['probability'] * 100
        # apply threshold
        if (probability >= probabilityThreshold):
            strSortedPreds = strSortedPreds + tagName + ": " + str(probability) + "\n"
  return strSortedPreds

# instantiate flask app and push a context
app = Flask(__name__)

# init camera
execution_path = os.getcwd()
camera = cv2.VideoCapture(1)
camera.set(cv2.CAP_PROP_FRAME_WIDTH,640)
camera.set(cv2.CAP_PROP_FRAME_HEIGHT,480)

while True:
    # Init and FPS process
    predSorted = ""
    start_time = time.time()
    fpsInfo = ""
    
    try:
        # Grab a single frame of video
        ret, frame = camera.read()
        fast_frame = cv2.resize(frame, (0, 0), fx=0.25, fy=0.25)

        # save image to disk and open it
        frameImageFileName = 'image.png'
        cv2.imwrite(frameImageFileName, fast_frame)

        with open(frameImageFileName, 'rb') as f:
            img_data = f.read()

        # analyze file in local container
        api_url = "http://127.0.0.1:8071/image"
        # analyze file in remote container RPI
        #api_url = "http://192.168.147.12:8080/image" #rpidev4
        #api_url = "http://192.168.137.12:8080/image" #rpidev5
        #api_url = "http://192.168.137.123:8080/image" #rpidev6
        api_url = "http://192.168.137.169:8080/image" #rpidev6
        r = requests.post(api_url, data=img_data)
        with app.app_context():
            jsonResults = jsonify(r.json())
        jsonStr = jsonResults.get_data(as_text=True)
        predSorted = getPredictionsSorted(jsonStr)

        # calculate FPS >> FPS = 1 / time to process loop
        fpsInfo = "FPS: " + str(1.0 / (time.time() - start_time)) + "\n-------------------\n" 

    except Exception as e:
        print('EXCEPTION:', str(e))

    # display FPS and Predictions, split text into lines, thanks OpenCV putText()
    frameInfo = fpsInfo + predSorted
    print(frameInfo)

    for i, line in enumerate(frameInfo.split('\n')):
        i = i + 1
        cv2.putText(frame, line, (10, 10 * i), cv2.FONT_HERSHEY_SIMPLEX, 0.4, (255, 255, 255), 1)

    # Display the resulting image
    cv2.imshow('Video', frame)

    # Hit 'q' on the keyboard to quit!
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

# Release handle to the webcam
camera.release()
cv2.destroyAllWindows()