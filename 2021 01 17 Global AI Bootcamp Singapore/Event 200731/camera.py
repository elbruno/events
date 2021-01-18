# -----------------------------------------------
# 30 CAMERA GENERAL CODE
# IMPORTS
# -----------------------------------------------

import socket
import time
import threading
import cv2

# -----------------------------------------------
# RECEIVE DATA FUNCTIONS
# -----------------------------------------------

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

# -----------------------------------------------
# SEND COMMAND  FUNCTIONS
# -----------------------------------------------

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
# CONNECTION TO THE DRONE
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

# -----------------------------------------------
# LISTENER THREADS
# -----------------------------------------------

# start threads
recThread = threading.Thread(target=receiveData)
recThread.daemon = True
recThread.start()

stateThread = threading.Thread(target=readStates)
stateThread.daemon = True
stateThread.start()

# -----------------------------------------------
# 31 CAMERA
# START DRONE CONNECTION
# -----------------------------------------------

# connect to drone
response = sendControlCommand("command")
print(f'command response: {response}')
response = sendControlCommand("streamon")
print(f'streamon response: {response}')

# -----------------------------------------------
# 32 CAMERA 
# START DRONE CAMERA
# -----------------------------------------------

# open UDP
print(f'opening UDP video feed, wait 2 seconds ')
videoUDP = 'udp://192.168.10.1:11111'
cap = cv2.VideoCapture(videoUDP)
time.sleep(2)

# -----------------------------------------------
# 35 CAMERA
# APP DISPLAY CAMERA WITH OPENCV and FPS
# -----------------------------------------------

# drone information
battery = 0

# open
i = 0
while True:
    i = i + 1
    start_time = time.time()

    sendReadCommand('battery?')
    print(f'battery: {battery} % - i: {i}')

    try:
        ret, frame = cap.read()
        img = cv2.resize(frame, (640, 480))

        if (time.time() - start_time ) > 0:
            fpsInfo = "FPS: " + str(1.0 / (time.time() - start_time)) # FPS = 1 / time to process loop
            font = cv2.FONT_HERSHEY_DUPLEX
            cv2.putText(img, fpsInfo, (10, 40), font, 0.4, (255, 255, 255), 1)

        cv2.imshow('@elbruno - DJI Tello Camera', img)
    except Exception as e:
        print(f'exc: {e}')
        pass

    if cv2.waitKey(1) & 0xFF == ord('q'):
        break
    
# -----------------------------------------------
# 34 CAMERA
# END STREAM AND CLOSE
# -----------------------------------------------

response = sendControlCommand("streamoff")
print(f'streamon response: {response}')