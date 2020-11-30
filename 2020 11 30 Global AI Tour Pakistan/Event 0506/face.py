# -----------------------------------------------
# 40 FACE DETECTION
# COMMON CODE
# -----------------------------------------------

# Bruno Capuano
# detect faces using haar cascades from https://github.com/opencv/opencv/tree/master/data/haarcascades
# enable drone video camera 
# display video camera using OpenCV
# display FPS
# detect faces

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

# open UDP
print(f'opening UDP video feed, wait 2 seconds ')
videoUDP = 'udp://192.168.10.1:11111'
cap = cv2.VideoCapture(videoUDP)
time.sleep(2)
vid_cod = cv2.VideoWriter_fourcc(*'XVID')
vid_output = cv2.VideoWriter("videos/cam_video.mp4", vid_cod, 20.0, (640,480))

# drone information
battery = 0

# -----------------------------------------------
# 41 FACE DETECTION
# FACE CASCADE
# -----------------------------------------------

# enable face detection
face_cascade = cv2.CascadeClassifier('haarcascade_frontalface_default.xml')

# -----------------------------------------------
# 42 FACE DETECTION
# DISPLAY FACES
# -----------------------------------------------

# open
i = 0
while True:
    i = i + 1

    try:
        _, frameOrig = cap.read()
        frame = cv2.resize(frameOrig, (640, 480))
        

        gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
        faces = face_cascade.detectMultiScale(gray, 1.3, 5)

        for (top, right, bottom, left) in faces:
            cv2.rectangle(frame,(top,right),(top+bottom,right+left),(0,0,255),2)

        cv2.imshow('@elbruno - DJI Tello Camera', frame)
        vid_output.write(img)

        sendReadCommand('battery?')
        print(f'battery: {battery} %')

    except Exception as e:
        print(f'exc: {e}')
        pass

    if cv2.waitKey(1) & 0xFF == ord('q'):
        break    

response = sendControlCommand("streamoff")
print(f'streamon response: {response}')
cap.release()
vid_output.release()
