# Bruno Capuano
# detect QR Codes using OpenCV based on https://github.com/cuicaihao/Webcam_QR_Detector/blob/master/Lab_01_QR_Bar_Code_Detector_Basic.ipynb
# generate sample QR Codes with Bing QR Code Generator https://www.bing.com/search?q=qrcode+generator&cvid=a50d3a94ef624baa8102bd6d4c68d1c3&FORM=ANAB01&PC=U531
# enable drone video camera 
# display video camera using OpenCV
# display FPS
# detect QR Codes

import cv2
import socket
import time
import threading
import numpy as np  
from pyzbar.pyzbar import decode

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

# QR Code Detector
qrDecoder = cv2.QRCodeDetector()

# open UDP
print(f'opening UDP video feed, wait 2 seconds ')
videoUDP = 'udp://192.168.10.1:11111'
cap = cv2.VideoCapture(videoUDP)
time.sleep(2)

# open
i = 0
detectionEnabled = True
while True:
    i = i + 1
    start_time = time.time()

    #try:
    _, frameOrig = cap.read()
    frame = cv2.resize(frameOrig, (640, 420))

    # detect faces
    if(detectionEnabled == True):
        for code in decode(frame):
            points = code.polygon
            # If the points do not form a quad, find convex hull
            if len(points) > 4 : 
                hull = cv2.convexHull(np.array([point for point in points], dtype=np.float32))
                hull = list(map(tuple, np.squeeze(hull)))
            else : 
                hull = points
            # Number of points in the convex hull
            n = len(hull)     
            # Draw the convext hull
            for j in range(0,n):
                cv2.line(frame, hull[j], hull[ (j+1) % n], (255,0,0), 3)
            x = code.rect.left
            y = code.rect.top                
            barCode = str(code.data)
            cv2.putText(frame, barCode, (x, y-10), cv2.FONT_HERSHEY_SIMPLEX, 0.5, (0,0,255), 1, cv2.LINE_AA)


    # display fps
    if (time.time() - start_time ) > 0:
        fpsInfo = "FPS: " + str(1.0 / (time.time() - start_time)) # FPS = 1 / time to process loop
        font = cv2.FONT_HERSHEY_DUPLEX
        cv2.putText(frame, fpsInfo, (10, 20), font, 0.4, (255, 255, 255), 1)

    cv2.imshow('@elbruno - DJI Tello Camera', frame)

    sendReadCommand('battery?')
    print(f'battery: {battery} % - i: {i} - {fpsInfo}')

    # except Exception as e:
    #     print(f'exc: {e}')
    #     pass

    # key controller
    key = cv2.waitKey(1) & 0xFF    
    if key == ord("d"):
        detectionEnabled = not detectionEnabled

    if key == ord("q"):
        break

response = sendControlCommand("streamoff")
print(f'streamon response: {response}')