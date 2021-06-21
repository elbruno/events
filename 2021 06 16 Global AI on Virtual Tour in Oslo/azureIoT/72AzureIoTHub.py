# -----------------------------------------------
# 70 AZURE IOT HUB Sample
# -----------------------------------------------

import socket
import time
import threading
import cv2
import iotc
import provision_service
import asyncio
import json
import datetime

from drone_device import Drone_Device

# -----------------------------------------------
# RECEIVE DATA FUNCTIONS
# -----------------------------------------------

def receiveData():
    global response, clientSocket
    while True:
        try:
            response, _ = clientSocket.recvfrom(1024)
        except:
            break

def readStates():
    global battery, agx, agy, agz, temph, templ
    global response, response_state, clientSocket, stateSocket, address

    while True:
        try:
            response_state, _ = stateSocket.recvfrom(256)
            if response_state != 'ok':
                response_state = response_state.decode('ASCII')
                list = response_state.replace(';', ':').split(':')
                # print(list)
                # print(battery)            
                # print(f'agx: {list[27]} ')
                # print(f'agy: {list[29]} ')
                # print(f'agz: {list[31]} ')
                # print(f'temph: {list[15]} ')
                # print(f'templ: {list[13]} ')
                battery = int(list[21])
                agx     = float(list[27])
                agy     = float(list[29])
                agz     = float(list[31])
                temph   = int(list[15])
                templ   = int(list[13])                
        # except:
        #     break
        except Exception as e:
            print(f'exc: {e}')
            pass

# -----------------------------------------------
# SEND COMMAND  FUNCTIONS
# -----------------------------------------------

def sendCommand(command):
    global response, clientSocket, address
    timestamp = int(time.time() * 1000)

    clientSocket.sendto(command.encode('utf-8'), address)

    while response is None:
        if (time.time() * 1000) - timestamp > 5 * 1000:
            return False

    return response


def sendReadCommand(command):
    global response
    response = sendCommand(command)
    try:
        response = str(response)
    except:
        pass
    return response

def sendControlCommand(command):
    global response, response_state, clientSocket, stateSocket, address
    response = None
    for i in range(0, 5):
        response = sendCommand(command)
        if response == 'OK' or response == 'ok':
            return True
    return False


# -----------------------------------------------
# AZURE IOT CENTRAL
# -----------------------------------------------

async def init_drone_AzureIoT():
    global drone

    iothub    = ""
    scope     = ""
    device_id = ""
    key       = ""
    drone = Drone_Device(scope, device_id, key)
    await drone.init_azureIoT()

# -----------------------------------------------
# 35 CAMERA
# APP DISPLAY CAMERA WITH OPENCV and FPS
# -----------------------------------------------

async def main():
    global battery, agx, agy, agz, temph, templ
    global response, response_state, clientSocket, stateSocket, address
    global drone

    await init_drone_AzureIoT()

    # CONNECTION TO THE DRONE
    
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

    # LISTENER THREADS

    # start threads
    recThread = threading.Thread(target=receiveData)
    recThread.daemon = True
    recThread.start()

    stateThread = threading.Thread(target=readStates)
    stateThread.daemon = True
    stateThread.start()

    # START DRONE CONNECTION

    # connect to drone
    response = sendControlCommand("command")
    print(f'command response: {response}')
    response = sendControlCommand("streamon")
    print(f'streamon response: {response}')

    # START DRONE CAMERA

    # open UDP
    print(f'opening UDP video feed, wait 2 seconds ')
    videoUDP = 'udp://192.168.10.1:11111'
    cap = cv2.VideoCapture(videoUDP)
    time.sleep(2)

    # drone information
    battery = 0
    agx     = 0
    agy     = 0
    agz     = 0
    temph   = 0
    templ   = 0

    # open
    i = 0
    while True:
        i = i + 1
        start_time = time.time()

        sendReadCommand('battery?')
        print(f'i: {i} - battery: {battery} % - agx: {agx} - agy: {agy} - agz: {agz} - temph: {temph} - templ: {templ}')

        await drone.send_telemetry(agx, agy, agz)

        if (i % 10) == 0:
            await drone.send_properties(temph, templ, battery)

        try:
            ret, frame = cap.read()
            img = cv2.resize(frame, (320, 240))

            if (time.time() - start_time ) > 0:
                fpsInfo = "FPS: " + str(1.0 / (time.time() - start_time)) # FPS = 1 / time to process loop
                font = cv2.FONT_HERSHEY_DUPLEX
                cv2.putText(img, fpsInfo, (10, 40), font, 0.4, (255, 255, 255), 1)

            cv2.imshow('@elbruno - DJI Tello Azure IoT', img)
        except Exception as e:
            print(f'exc: {e}')
            pass

        if cv2.waitKey(1) & 0xFF == ord('q'):
            break

        time.sleep(1)

    # -----------------------------------------------
    # 34 CAMERA
    # END STREAM AND CLOSE
    # -----------------------------------------------

    response = sendControlCommand("streamoff")
    print(f'streamon response: {response}')    

if __name__ == "__main__":
    asyncio.run(main())
