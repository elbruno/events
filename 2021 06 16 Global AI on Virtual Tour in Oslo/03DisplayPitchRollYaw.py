# Bruno Capuano
# get drone battery demo

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
    global pitch
    global roll
    global yaw
    while True:
        try:
            response_state, _ = stateSocket.recvfrom(256)
            if response_state != 'ok':
                response_state = response_state.decode('ASCII')
                list = response_state.replace(';', ':').split(':')
                battery = int(list[21])                
                pitch = int(list[1])                
                roll = int(list[3])                
                yaw = int(list[5])                
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

# drone information
battery = 0
pitch = 0
roll = 0
yaw = 0

# connect to drone
sendControlCommand("command")

# get and display battery
sendReadCommand('battery?')

i = 0
while i < 50:
    i = i + 1
    time.sleep(1)
    pitchStr = str(pitch).zfill(3)
    rollStr = str(roll).zfill(3)
    yawStr = str(yaw).zfill(3)
    print(f'Battery: {battery} % - Pitch: {pitchStr} - Roll: {rollStr} - Yaw: {yawStr} - i:{i}')