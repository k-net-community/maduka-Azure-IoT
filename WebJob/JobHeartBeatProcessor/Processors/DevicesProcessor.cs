﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobHeartBeatProcessor.Processors
{
    using JobHeartBeatProcessor.Models;
    using Microsoft.Azure.Devices;

    class DevicesProcessor
    {
        private List<DeviceEntity> listOfDevices;
        private RegistryManager registryManager;
        private String iotHubConnectionString;
        private int maxCountOfDevices;
        private String protocolGatewayHostName;

        public DevicesProcessor(string iotHubConnenctionString, int devicesCount, string protocolGatewayName)
        {
            this.listOfDevices = new List<DeviceEntity>();
            this.iotHubConnectionString = iotHubConnenctionString;
            this.maxCountOfDevices = devicesCount;
            this.protocolGatewayHostName = protocolGatewayName;
            this.registryManager = RegistryManager.CreateFromConnectionString(iotHubConnectionString);
        }

        public async Task<List<DeviceEntity>> GetDevices()
        {
            try
            {
                DeviceEntity deviceEntity;
                var devices = await registryManager.GetDevicesAsync(maxCountOfDevices);

                if (devices != null)
                {
                    foreach (var device in devices)
                    {
                        deviceEntity = new DeviceEntity()
                        {
                            Id = device.Id,
                            ConnectionState = device.ConnectionState.ToString(),
                            // 暫不取得每一個裝置的ConnectionString，所以先標示為註解
                            // 若是有需要的話，取消這行的標記以及下方CreateDeviceConnectionString副程式的註解標記
                            // ConnectionString = CreateDeviceConnectionString(device),
                            LastActivityTime = device.LastActivityTime,
                            LastConnectionStateUpdatedTime = device.ConnectionStateUpdatedTime,
                            LastStateUpdatedTime = device.StatusUpdatedTime,
                            MessageCount = device.CloudToDeviceMessageCount,
                            State = device.Status.ToString(),
                            SuspensionReason = device.StatusReason
                        };

                        if (device.Authentication != null &&
                            device.Authentication.SymmetricKey != null)
                        {
                            deviceEntity.PrimaryKey = device.Authentication.SymmetricKey.PrimaryKey;
                            deviceEntity.SecondaryKey = device.Authentication.SymmetricKey.SecondaryKey;
                        }

                        listOfDevices.Add(deviceEntity);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return listOfDevices;
        }
    }
}
