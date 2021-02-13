# Bruno Capuano
# detect faces using a DNN model 
# download model and prototxt from https://github.com/spmallick/learnopencv/tree/master/FaceDetectionComparison/models
# enable drone video camera 
# display video camera using OpenCV and display FPS
# detect faces
# launch the drone with key T, and land with key L
# if the drone is flying, and a face is detected, the drone will flip left

import cv2
import socket
import time
import threading
import winsound

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

# -----------------------------------------------
# Face Detection using DNN Net
# -----------------------------------------------

def detectFaceOpenCVDnn(net, frame, conf_threshold=0.7):
    
    frameHeight = frame.shape[0]
    frameWidth = frame.shape[1]
    blob = cv2.dnn.blobFromImage(frame, 1.0, (300, 300), [104, 117, 123], False, False,)

    net.setInput(blob)
    detections = net.forward()
    bboxes = []
    for i in range(detections.shape[2]):
        confidence = detections[0, 0, i, 2]
        if confidence > conf_threshold:
            x1 = int(detections[0, 0, i, 3] * frameWidth)
            y1 = int(detections[0, 0, i, 4] * frameHeight)
            x2 = int(detections[0, 0, i, 5] * frameWidth)
            y2 = int(detections[0, 0, i, 6] * frameHeight)
            bboxes.append([x1, y1, x2, y2])
            cv2.rectangle(frame, (x1, y1), (x2, y2), (0, 255, 0), int(round(frameHeight / 150)), 8,)
    return frame, bboxes

# drone information
battery = 0
flyUnit = 50

# load face detection model
modelFile = "models/res10_300x300_ssd_iter_140000_fp16.caffemodel"
configFile = "models/deploy.prototxt"
net = cv2.dnn.readNetFromCaffe(configFile, modelFile)

# open UDP
print(f'opening UDP video feed, wait 2 seconds ')
videoUDP = 'udp://192.168.10.1:11111'
cap = cv2.VideoCapture(videoUDP)
time.sleep(2)

# -----------------------------------------------
# Main program
# -----------------------------------------------

# open
drone_flying = False
i = 0
while True:
    i = i + 1
    start_time = time.time()

    try:
        _, frameOrig = cap.read()
        frame = cv2.resize(frameOrig, (480, 360))

        # detect faces
        if(detectionEnabled == True):
            outOpencvDnn, bboxes = detectFaceOpenCVDnn(net, frame)
            if(len(bboxes) > 0 and drone_flying == True):
                msg = "flip l"
                sendCommand(msg)

        # display fps
        if (time.time() - start_time ) > 0:
            fpsInfo = "FPS: " + str(1.0 / (time.time() - start_time)) # FPS = 1 / time to process loop
            font = cv2.FONT_HERSHEY_DUPLEX
            cv2.putText(frame, fpsInfo, (10, 20), font, 0.4, (255, 255, 255), 1)

        cv2.imshow('@elbruno - Face and Flip', frame)

        sendReadCommand('battery?')
        print(f'flying: {drone_flying} - battery: {battery} % - i: {i} - {fpsInfo}')

    except Exception as e:
        print(f'exc: {e}')
        pass

    if cv2.waitKey(1) & 0xFF == ord('t'):
        drone_flying = True
        detection_started = True
        msg = "takeoff"
        sendCommand(msg)

    if cv2.waitKey(1) & 0xFF == ord('l'):
        drone_flying = False
        msg = "land"
        sendCommand(msg)
        time.sleep(5)
    
    if (cv2.waitKey(1) & 0xFF == ord('w')) and drone_flying == True:
        msg = str(f"up {flyUnit}")
        sendCommand(msg)
        time.sleep(1)

    if (cv2.waitKey(1) & 0xFF == ord('s')) and drone_flying == True:
        msg = str(f"down {flyUnit}")
        sendCommand(msg)
        time.sleep(1)

    if (cv2.waitKey(1) & 0xFF == ord('a')) and drone_flying == True:
        msg = str(f"left {flyUnit}")
        sendCommand(msg)
        time.sleep(1)

    if (cv2.waitKey(1) & 0xFF == ord('d')) and drone_flying == True:
        msg = str(f"right {flyUnit}")
        sendCommand(msg)
        time.sleep(1)

    if (cv2.waitKey(1) & 0xFF == ord('r')) and drone_flying == True:
        msg = str(f"forward {flyUnit}")
        sendCommand(msg)
        time.sleep(1)

    if (cv2.waitKey(1) & 0xFF == ord('f')) and drone_flying == True:
        msg = str(f"back {flyUnit}")
        sendCommand(msg)
        time.sleep(1)

    if cv2.waitKey(1) & 0xFF == ord('q'):
        break    

# land and cleanup resources
msg = "land"
sendCommand(msg) # land
response = sendControlCommand("streamoff")
print(f'streamon response: {response}')