# Bruno Capuano
# open camera with openCV
# analyze camera frame with local docker custom vision project
# draw bounding boxes for each reconized object

import socket
import time
import threading
import cv2
import urllib
import json
import requests
import os
from flask import Flask, request, jsonify


def receiveData():
    global response
    while True:
        try:
            response, _ = clientSocket.recvfrom(1024)
        except:
            break

def readStates():
    global battery
    while True:
        try:
            response_state, _ = stateSocket.recvfrom(256)
            if response_state != 'ok':
                response_state = response_state.decode('ASCII')
                list = response_state.replace(';', ':').split(':')
                battery = int(list[21])                
                pitch = int(list[1])                                
        except:
            break

def sendCommand(command):
    global response
    timestamp = int(time.time() * 1000)

    clientSocket.sendto(command.encode('utf-8'), address)

    while response is None:
        if (time.time() * 1000) - timestamp > 5 * 1000:
            return False

    return response


def sendReadCommand(command):
    response = sendCommand(command)
    try:
        response = str(response)
    except:
        pass
    return response

def sendControlCommand(command):
    response = None
    for i in range(0, 5):
        response = sendCommand(command)
        if response == 'OK' or response == 'ok':
            return True
    return False

# -----------------------------------------------
# Local calls
# -----------------------------------------------

probabilityThreshold = 75

def displayPredictions(jsonPrediction, frame):
    global camera_Width, camera_Heigth
    jsonObj = json.loads(jsonPrediction)
    preds = jsonObj['predictions']
    sorted_preds = sorted(preds, key=lambda x: x['probability'], reverse=True)
    strSortedPreds = ""
    resultFound = False
    if (sorted_preds):
        for pred in sorted_preds:
            # tag name and prob * 100
            tagName     = str(pred['tagName'])
            probability = pred['probability'] * 100
            # apply threshold
            if (probability >= probabilityThreshold):
                bb = pred['boundingBox']

                # adjust to size
                height = int(bb['height'] * camera_Heigth)
                left = int(bb['left'] * camera_Width)
                top = int(bb['top'] * camera_Heigth)
                width = int(bb['width'] * camera_Width)

                # draw bounding boxes
                start_point = (top, left)                 
                end_point = (top + height, left + width) 
                color = (255, 0, 0) 
                thickness = 2                
                cv2.rectangle(img, start_point, end_point, color, thickness)                 

                print(f'MVP - {probability}')
                print(f'start point: {start_point} - end point: {end_point}')
                print(jsonPrediction)
                
    return strSortedPreds

# instantiate flask app and push a context
app = Flask(__name__)

# -----------------------------------------------
# Main program
# -----------------------------------------------

# connection info
UDP_IP = '192.168.10.1'
UDP_PORT = 8889
last_received_command = time.time()
STATE_UDP_PORT = 8890

address = (UDP_IP, UDP_PORT)
response = None
response_state = None

clientSocket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
clientSocket.bind(('', UDP_PORT))
stateSocket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
stateSocket.bind(('', STATE_UDP_PORT))

# start threads
recThread = threading.Thread(target=receiveData)
recThread.daemon = True
recThread.start()

stateThread = threading.Thread(target=readStates)
stateThread.daemon = True
stateThread.start()

# connect to drone
response = sendControlCommand("command")
print(f'command response: {response}')
response = sendControlCommand("streamon")
print(f'streamon response: {response}')

# drone information
battery = 0
pitch = 0

# open UDP
print(f'opening UDP video feed, wait 2 seconds ')
videoUDP = 'udp://192.168.10.1:11111'
cap = cv2.VideoCapture(videoUDP)
time.sleep(2)
camera_Width = 640
camera_Heigth = 480

# open
i = 0
while True:
    i = i + 1
    imgNumber = str(i).zfill(5)
    start_time = time.time()

    sendReadCommand('battery?')
    print(f'battery: {battery} % - pitch: {pitch} - i: {imgNumber}')

    try:
        ret, frame = cap.read()
        img = cv2.resize(frame, (camera_Width, camera_Heigth))
        
        # save image to disk and open it
        frameImageFileName = str(f'tmp\image{imgNumber}.png')
        cv2.imwrite(frameImageFileName, img)

        with open(frameImageFileName, 'rb') as f:
            img_data = f.read()

        # analyze file in local container
        api_url = "http://127.0.0.1:8070/image"
        r = requests.post(api_url, data=img_data)
        with app.app_context():
            jsonResults = jsonify(r.json())
        jsonStr = jsonResults.get_data(as_text=True)
        displayPredictions(jsonStr, frame)
        
        fpsInfo = ""
        if (time.time() - start_time ) > 0:
            fpsInfo = "FPS: " + str(1.0 / (time.time() - start_time)) # FPS = 1 / time to process loop
            font = cv2.FONT_HERSHEY_DUPLEX
            cv2.putText(img, fpsInfo, (10, 20), font, 0.4, (255, 255, 255), 1)

        cv2.imshow('@elbruno - DJI Tello Camera', img)
    except Exception as e:
        print(f'exc: {e}')
        pass

    if cv2.waitKey(1) & 0xFF == ord('q'):
        break
    

response = sendControlCommand("streamoff")
print(f'streamon response: {response}')