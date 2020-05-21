# -----------------------------------------------
# 1 COMMON CODE
# -----------------------------------------------

# Bruno Capuano
# enable drone video camera 
# display video camera using OpenCV
# display FPS

from tensorflow.keras.applications.mobilenet_v2 import preprocess_input
from tensorflow.keras.preprocessing.image import img_to_array
from tensorflow.keras.models import load_model
import numpy as np
import time
import socket
import time
import threading
import cv2

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

def detect_and_predict_mask(frame, faceNet, maskNet):
    (h, w) = frame.shape[:2]
    blob = cv2.dnn.blobFromImage(frame, 1.0, (300, 300), (104.0, 177.0, 123.0))

    faceNet.setInput(blob)
    detections = faceNet.forward()

    faces = []
    locs = []
    preds = []

    for i in range(0, detections.shape[2]):
        confidence = detections[0, 0, i, 2]

        if confidence > confidenceThreshold:
            box = detections[0, 0, i, 3:7] * np.array([w, h, w, h])
            (startX, startY, endX, endY) = box.astype("int")

            (startX, startY) = (max(0, startX), max(0, startY))
            (endX, endY) = (min(w - 1, endX), min(h - 1, endY))

            face = frame[startY:endY, startX:endX]
            face = cv2.cvtColor(face, cv2.COLOR_BGR2RGB)
            face = cv2.resize(face, (224, 224))
            face = img_to_array(face)
            face = preprocess_input(face)
            face = np.expand_dims(face, axis=0)

            faces.append(face)
            locs.append((startX, startY, endX, endY))

    if len(faces) > 0:
        preds = maskNet.predict(faces)

    return (locs, preds)

# -----------------------------------------------
# Drone Connection Information
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

# -----------------------------------------------
# Connect to drone Camera
# -----------------------------------------------

# open UDP
print(f'opening UDP video feed, wait 2 seconds ')
videoUDP = 'udp://192.168.10.1:11111'
cap = cv2.VideoCapture(videoUDP)
time.sleep(2)
vid_cod = cv2.VideoWriter_fourcc(*'XVID')
vid_output = cv2.VideoWriter("videos/cam_video.mp4", vid_cod, 20.0, (640,480))

# -----------------------------------------------
# 2 LOAD FACE DETECTOR
# -----------------------------------------------

# load the face mask detector model from disk
prototxtPath = "deploy.prototxt"
weightsPath = "res10_300x300_ssd_iter_140000.caffemodel"
faceNet = cv2.dnn.readNet(prototxtPath, weightsPath)
maskNet = load_model("mask_detector.model")
confidenceThreshold = 0.5
maskDetectionEnabled = False

# -----------------------------------------------
# Image Configurations
# -----------------------------------------------
width = 640
height = 480
dsize = (width, height)

# load bottom img
background = cv2.imread('Bottom03.png')
background = cv2.resize(background, dsize)

# -----------------------------------------------
# 3 MAIN APP
# -----------------------------------------------

# open
i = 0
while True:
    i = i + 1
    start_time = time.time()

    sendReadCommand('battery?')
    print(f'battery: {battery} % - i: {i}')

    try:
        ret, frameOrig = cap.read()
        frame = cv2.resize(frameOrig, dsize)
        
        (locs, preds) = detect_and_predict_mask(frame, faceNet, maskNet)
        for (box, pred) in zip(locs, preds):
            (startX, startY, endX, endY) = box
            (mask, withoutMask) = pred

            label = "Mask" if mask > withoutMask else "No Mask"
            color = (0, 255, 0) if label == "Mask" else (0, 0, 255)

            label = "{}: {:.2f}%".format(label, max(mask, withoutMask) * 100)

            cv2.putText(frame, label, (startX, endY + 10),
            cv2.FONT_HERSHEY_DUPLEX, 0.40, color, 1)
            cv2.rectangle(frame, (startX, startY), (endX, endY), color, 2)

        if (time.time() - start_time ) > 0:
            fpsInfo = "FPS: " + str(1.0 / (time.time() - start_time)) # FPS = 1 / time to process loop
            font = cv2.FONT_HERSHEY_DUPLEX
            cv2.putText(frame, fpsInfo, (10, 20), font, 0.4, (255, 255, 255), 1)

        frameAndLogo = cv2.addWeighted(background, 1, frame, 1, 0)

        cv2.imshow('@elbruno - DJI Tello Camera', frameAndLogo)
        vid_output.write(frame)
    except Exception as e:
        print(f'exc: {e}')
        pass

    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

# -----------------------------------------------
# 4 DISPOSE
# -----------------------------------------------

response = sendControlCommand("streamoff")
print(f'streamon response: {response}')

# close the already opened camera, and the video file
cap.release()
vid_output.release()
cv2.destroyAllWindows()