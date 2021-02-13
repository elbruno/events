# Bruno Capuano
# detect faces using haar cascades from https://github.com/opencv/opencv/tree/master/data/haarcascades
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

# drone information
battery = 0
flyUnit = 50

# enable face and smile detection
face_cascade = cv2.CascadeClassifier('haarcascade_frontalface_default.xml')

# open UDP
print(f'opening UDP video feed, wait 2 seconds ')
videoUDP = 'udp://192.168.10.1:11111'
cap = cv2.VideoCapture(videoUDP)
time.sleep(2)

# open
drone_flying = False
i = 0
while True:
    i = i + 1
    start_time = time.time()

    try:
        _, frameOrig = cap.read()
        frame = cv2.resize(frameOrig, (480, 360))
        gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)

        # detect faces
        faces = face_cascade.detectMultiScale(gray, 1.3, 5)
        for (x, y, w, h) in faces: 
            cv2.rectangle(frame, (x, y), ((x + w), (y + h)), (0, 0, 255), 2) 

            font = cv2.FONT_HERSHEY_COMPLEX_SMALL
            cv2.putText(frame, 'face', (h + 6, w - 6), font, 0.7, (255, 255, 255), 1)

        if(len(faces) > 0 and drone_flying == True):
            msg = "flip l"
            sendCommand(msg)

        # display fps
        if (time.time() - start_time ) > 0:
            fpsInfo = "FPS: " + str(1.0 / (time.time() - start_time)) # FPS = 1 / time to process loop
            font = cv2.FONT_HERSHEY_DUPLEX
            cv2.putText(frame, fpsInfo, (10, 20), font, 0.4, (255, 255, 255), 1)

        cv2.imshow('@elbruno - DJI Tello Camera', frame)

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