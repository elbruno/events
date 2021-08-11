# Copyright (c) 2020 Avanade
# Author: Thor Schueler
#
# Permission is hereby granted, free of charge, to any person obtaining a copy
# of this software and associated documentation files (the "Software"), to deal
# in the Software without restriction, including without limitation the rights
# to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
# copies of the Software, and to permit persons to whom the Software is
# furnished to do so, subject to the following conditions:
#
# The above copyright notice and this permission notice shall be included in
# all copies or substantial portions of the Software.
#
# THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
# IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
# FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
# AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
# LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
# OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
# THE SOFTWARE.
#
import asyncio
import os
import datetime
from azure.iot.device.aio import ProvisioningDeviceClient


class Device():
    def __init__(self, scope, device_id, key, iothub):
        self.scope = scope
        self.device_id = device_id
        self.key = key
        self.iothub = iothub

    async def __register_device(self):
        provisioning_device_client = ProvisioningDeviceClient.create_from_symmetric_key(
            provisioning_host='global.azure-devices-provisioning.net',
            registration_id=self.device_id,
            id_scope=self.scope,
            symmetric_key=self.key,
        )

        return await provisioning_device_client.register()

    @property
    async def connection_string(self):
        if(self.iothub == None or self.iothub == ""):
            print(f'{datetime.datetime.now()}: No IOTHUB specified. Attempting to resolve via global.azure-devices-provisioning.net')
            results = await asyncio.gather(self.__register_device())
            print(results)
            registration_result = results[0]

            # build the connection string
            conn_str = 'HostName=' + registration_result.registration_state.assigned_hub + \
                ';DeviceId=' + self.device_id + \
                ';SharedAccessKey=' + self.key
        else:
            conn_str = 'HostName=' + self.iothub + \
                ';DeviceId=' + self.device_id + \
                ';SharedAccessKey=' + self.key

        # if (os.environ["DEBUG"] == 'TRUE'):
        print(f'{datetime.datetime.now()}: Connection String = {conn_str}')

        return conn_str
