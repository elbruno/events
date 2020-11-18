# Bruno Capuano
# detect faces, age and gender using models from https://github.com/spmallick/learnopencv/tree/08e61fe80b8c0244cc4029ac11e44cd0fbb008c3/AgeGender
# enable drone video camera 
# display video camera using OpenCV
# display FPS
# detect faces, and detect age and gender for each detected face

import cv2
import socket
import time
import threading

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
# Face, Age and Gender Detection
# -----------------------------------------------

def getFaceBox(net, frame, conf_threshold=0.7):    
    frameHeight = frame.shape[0]
    frameWidth = frame.shape[1]
    blob = cv2.dnn.blobFromImage(frame, 1.0, (300, 300), [104, 117, 123], True, False)

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
            cv2.rectangle(frame, (x1, y1), (x2, y2), (0, 255, 0), int(round(frameHeight/150)), 8)
    return frame, bboxes

def showAgeAndGenderBoxes(genderNet, ageNet, face, frame):

    blob = cv2.dnn.blobFromImage(face, 1.0, (227, 227), MODEL_MEAN_VALUES, swapRB=False)
    genderNet.setInput(blob)
    genderPreds = genderNet.forward()
    gender = genderList[genderPreds[0].argmax()]

    ageNet.setInput(blob)
    agePreds = ageNet.forward()
    age = ageList[agePreds[0].argmax()]

    label = "{},{}".format(gender, age)
    cv2.putText(frame, label, (bbox[0], bbox[1]-10), cv2.FONT_HERSHEY_SIMPLEX, 0.8, (0, 255, 255), 2, cv2.LINE_AA)

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

# load face, age and gender detection models
faceProto = "modelsagegender/opencv_face_detector.pbtxt"
faceModel = "modelsagegender/opencv_face_detector_uint8.pb"
ageProto = "modelsagegender/age_deploy.prototxt"
ageModel = "modelsagegender/age_net.caffemodel"
genderProto = "modelsagegender/gender_deploy.prototxt"
genderModel = "modelsagegender/gender_net.caffemodel"

MODEL_MEAN_VALUES = (78.4263377603, 87.7689143744, 114.895847746)
ageList = ['(0-2)', '(4-6)', '(8-12)', '(15-20)', '(25-32)', '(38-43)', '(48-53)', '(60-100)']
genderList = ['Male', 'Female']

ageNet = cv2.dnn.readNet(ageModel, ageProto)
genderNet = cv2.dnn.readNet(genderModel, genderProto)
faceNet = cv2.dnn.readNet(faceModel, faceProto)

# ageNet.setPreferableBackend(cv2.dnn.DNN_BACKEND_CUDA)
# ageNet.setPreferableTarget(cv2.dnn.DNN_TARGET_CUDA)
# genderNet.setPreferableBackend(cv2.dnn.DNN_BACKEND_CUDA)
# genderNet.setPreferableTarget(cv2.dnn.DNN_TARGET_CUDA)
# faceNet.setPreferableBackend(cv2.dnn.DNN_BACKEND_CUDA)
# faceNet.setPreferableTarget(cv2.dnn.DNN_TARGET_CUDA)

# open UDP
print(f'opening UDP video feed, wait 2 seconds ')
videoUDP = 'udp://192.168.10.1:11111'
cap = cv2.VideoCapture(videoUDP)
time.sleep(2)

# open
i = 0
padding = 20
detectionEnabled = False
while True:
    i = i + 1
    start_time = time.time()

    try:
        _, frameOrig = cap.read()
        frame = cv2.resize(frameOrig, (640, 420))

        # detect faces
        if(detectionEnabled == True):
            outOpencvDnn, bboxes = getFaceBox(faceNet, frame)

            for bbox in bboxes:
                # get face frame
                face = frame[max(0,bbox[1]-padding):min(bbox[3]+padding,frame.shape[0]-1),max(0,bbox[0]-padding):min(bbox[2]+padding, frame.shape[1]-1)]

                cv2.imshow('@elbruno - Face', face)

                # detect and show age and gender
                showAgeAndGenderBoxes(genderNet, ageNet, face, frame)

        # display fps
        if (time.time() - start_time ) > 0:
            fpsInfo = "FPS: " + str(1.0 / (time.time() - start_time)) # FPS = 1 / time to process loop
            font = cv2.FONT_HERSHEY_DUPLEX
            cv2.putText(frame, fpsInfo, (10, 20), font, 0.4, (255, 255, 255), 1)

        cv2.imshow('@elbruno - DJI Tello Camera', frame)

        sendReadCommand('battery?')
        print(f'battery: {battery} % - i: {i} - {fpsInfo}')

    except Exception as e:
        print(f'exc: {e}')
        pass

    # key controller
    key = cv2.waitKey(1) & 0xFF    
    if key == ord("d"):
        detectionEnabled = not detectionEnabled

    if key == ord("q"):
        break

response = sendControlCommand("streamoff")
print(f'streamon response: {response}')