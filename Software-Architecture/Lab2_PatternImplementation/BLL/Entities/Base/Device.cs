using HardwareSim.BLL.Abstractions;
using HardwareSim.BLL.Entities.Components;
using HardwareSim.BLL.Entities.Devices;
using HardwareSim.BLL.Features.Events;
using HardwareSim.BLL.Features.PowerManagement;
using HardwareSim.BLL.Features.SoftwareManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace HardwareSim.BLL.Entities.Base
{
    public abstract class Device : IDevice
    {
        public event EventHandler<HardwareEventArgs>? OnStateNotification;

        public bool IsConnectedToGrid { get; set; } = true;
        public bool HasNetworkConnection { get; set; } = true;

        public Memory DeviceMemory { get; set; } = new();
        public Processor? DeviceProcessor { get; set; }
        public Battery? DeviceBattery { get; set; }
        public Battery? DeviceUPS { get; set; }
        public abstract DevicePlatform Platform { get; }

        public List<HardwarePeripheral> ConnectedPeripherals { get; set; } = new();

        private readonly PowerService _powerService = new();
        private readonly SoftwareManager _softwareManager = new();

        // Event publisher
        protected void Notify(string message)
        {
            OnStateNotification?.Invoke(this, new HardwareEventArgs(message));
        }

        public void SetGridConnection(bool isConnected)
        {
            IsConnectedToGrid = isConnected;

            if (isConnected)
            {
                DeviceBattery?.Recharge();
                DeviceUPS?.Recharge();

                Notify("Device plugged into grid. Batteries are fully recharged.");
            }
            else
            {
                Notify("Grid power lost. Switching to backup power.");
            }
        }

        public void SetNetworkConnection(bool isConnected)
        {
            HasNetworkConnection = isConnected;
            Notify(isConnected ? "Device connected to the network." : "Network connection disabled.");
        }

        public void ConnectPeripheral(HardwarePeripheral peripheral)
        {
            ConnectedPeripherals.Add(peripheral);
            Notify($"{peripheral.Name} connected.");
        }

        public void DisconnectPeripheral(string peripheralName)
        {
            var peripheral = ConnectedPeripherals.FirstOrDefault(p => p.Name.ToLower() == peripheralName.ToLower());

            if (peripheral != null)
            {
                ConnectedPeripherals.Remove(peripheral);
                Notify($"{peripheral.Name} was safely disconnected.");
            }
            else
            {
                Notify($"Peripheral '{peripheralName}' is not currently connected.");
            }
        }

        public void ExecuteOperation(string softwareName, double durationHours)
        {
            // 1. Get the actual software object from Memory
            var software = DeviceMemory.GetSoftware(softwareName);
            if (software == null)
            {
                Notify($"Operation Failed: Software '{softwareName}' is not installed.");
                return;
            }

            bool hasAudio = ConnectedPeripherals.Any(p => p.IsAudioDevice);

            // 2. Validate prerequisites using the object's real properties
            if (!_softwareManager.ValidatePrerequisites(software, HasNetworkConnection, hasAudio, out string error))
            {
                Notify($"Operation Failed: {error}");
                return;
            }

            // 3. Consume power using the object's built-in IsIntensive property!
            if (_powerService.TryConsumePower(IsConnectedToGrid, DeviceBattery, DeviceUPS, durationHours, software.IsIntensive, out string powerMsg))
            {
                Notify($"Operation '{software.Name}' completed successfully. {powerMsg}");
            }
            else
            {
                Notify($"Power Failure: {powerMsg}");
            }
        }
    }
}