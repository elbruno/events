import os
import datetime
import asyncio
import json

from azure.iot.device.aio import IoTHubDeviceClient
from azure.iot.device import MethodResponse
from azure.iot.device.aio import ProvisioningDeviceClient

class Drone_Device():
    def __init__(self, scope, device_id, key, iothub : str = ""):
        self.scope = scope
        self.device_id = device_id
        self.key = key
        self.iothub = iothub

    async def init_azureIoT(self):
        cnn_str = await self.get_connection_string()
        self.device_client = IoTHubDeviceClient.create_from_connection_string(cnn_str)
        await self.device_client.connect()


    async def __register_device(self):
        provisioning_device_client = ProvisioningDeviceClient.create_from_symmetric_key(
            provisioning_host='global.azure-devices-provisioning.net',
            registration_id=self.device_id,
            id_scope=self.scope,
            symmetric_key=self.key,
        )

        return await provisioning_device_client.register()

    async def get_connection_string(self):
        if(self.iothub == None or self.iothub == ""):
            print(f'{datetime.datetime.now()}: No IOTHUB specified. Attempting to resolve via global.azure-devices-provisioning.net')
            results = await asyncio.gather(self.__register_device())
            print(results)
            registration_result = results[0]
            cnn_str = 'HostName=' + registration_result.registration_state.assigned_hub + \
                ';DeviceId=' + self.device_id + \
                ';SharedAccessKey=' + self.key
        else:
            cnn_str = 'HostName=' + self.iothub + \
                ';DeviceId=' + self.device_id + \
                ';SharedAccessKey=' + self.key
        print(f'{datetime.datetime.now()}: Connection String = {cnn_str}')

        return cnn_str

    async def send_telemetry(self, agx, agy, agz):
        try:
            payload:str = ""
            data = {
                "agx": agx,
                "agy": agy,
                "agz": agz
            }
            payload = json.dumps(data)
            print(f"{datetime.datetime.now()}: telemetry: {payload}")
            await self.device_client.send_message(payload)                

        except Exception as e:
            print(f"{datetime.datetime.now()}: Exception during sending metrics: {e}")

    async def send_properties(self, bat, temph, templ):
        try:
            data = {
                'bat': bat,
                'templ': templ,
                'temph': temph
            }
            propertiesToUpdate = data
            print(f"{datetime.datetime.now()}: properties: {propertiesToUpdate}")

            await self.device_client.patch_twin_reported_properties(propertiesToUpdate)

        except Exception as e:
            print(f"{datetime.datetime.now()}: Exception during sending metrics: {e}")