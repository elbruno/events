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

from azure.iot.device.aio import IoTHubDeviceClient
from azure.iot.device import MethodResponse

# -----------------------------------------------
# RECEIVE DATA FUNCTIONS
# -----------------------------------------------

def receiveData():
    global response, response_state, clientSocket, stateSocket, address
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
                print(list)
                battery = int(list[21])                
                agx     = int(list[24])
                agy     = int(list[25])
                agz     = int(list[26])
                temph   = int(list[18])
                templ   = int(list[17])                
        except:
            break

# -----------------------------------------------
# SEND COMMAND  FUNCTIONS
# -----------------------------------------------

def sendCommand(command):
    global response, response_state, clientSocket, stateSocket, address
    timestamp = int(time.time() * 1000)

    clientSocket.sendto(command.encode('utf-8'), address)

    while response is None:
        if (time.time() * 1000) - timestamp > 5 * 1000:
            return False

    return response


def sendReadCommand(command):
    global response, response_state, clientSocket, stateSocket, address
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
async def send_telemetry(agx, agy, agz, bat, temph, templ):
    global device, device_client
    global scope, device_id, key, iothub

    # Define behavior for sending telemetry
    try:
        payload:str = ""
        data = {
            "agx": agx,
            "agy": agy,
            "agz": agz
        }
        payload = json.dumps(data)
        print(f"{datetime.datetime.now()}: telemetry: {payload}")
        await device_client.send_message(payload)                

        data = {
            'bat': bat,
            'templ': templ,
            'temph': temph
        }
        propertiesToUpdate = data
        print(f"{datetime.datetime.now()}: properties: {propertiesToUpdate}")

        await device_client.patch_twin_reported_properties(propertiesToUpdate)

    except Exception as e:
        print(f"{datetime.datetime.now()}: Exception during sending metrics: {e}")
    # finally:
    #     await asyncio.sleep(sampleRateInSeconds)

async def init_azureIOT():
    global device, device_client
    global scope, device_id, key, iothub

    iothub    = ""
    scope     = ""
    device_id = ""
    key       = ""

    device = provision_service.Device(scope, device_id, key, iothub)
    connectionString = await device.connection_string
    device_client = IoTHubDeviceClient.create_from_connection_string(connectionString)
    await device_client.connect()

# -----------------------------------------------
# 35 CAMERA
# APP DISPLAY CAMERA WITH OPENCV and FPS
# -----------------------------------------------

async def main():
    global battery, agx, agy, agz, temph, templ
    global response, response_state, clientSocket, stateSocket, address
    
    await init_azureIOT()

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

        await send_telemetry(agx, agy, agz, battery, temph, templ)

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

    # -----------------------------------------------
    # 34 CAMERA
    # END STREAM AND CLOSE
    # -----------------------------------------------

    response = sendControlCommand("streamoff")
    print(f'streamon response: {response}')    

if __name__ == "__main__":
    asyncio.run(main())
