using System;
using System.Collections.Generic;
using System.Linq;
using HardwareSim.BLL.Abstractions;
using HardwareSim.BLL.Entities.Components;
using HardwareSim.BLL.Events;
using HardwareSim.BLL.Services;

namespace HardwareSim.BLL.Entities.Base
{
    public abstract class Device : IDevice
    {
        public event EventHandler<HardwareEventArgs>? OnStateNotification;

        public bool IsConnectedToGrid { get; private set; } = true;
        public bool HasNetworkConnection { get; set; } = true;

        public Memory DeviceMemory { get; } = new();
        public Processor? DeviceProcessor { get; protected set; }
        public IPowerSource DeviceBattery { get; protected set; }
        public IPowerSource DeviceUPS { get; protected set; }

        protected List<IPeripheral> ConnectedPeripherals { get; } = new();

        // Injecting services via Composition
        private readonly PowerService _powerService = new();
        private readonly SoftwareManager _softwareManager = new();

        protected void Notify(string message)
        {
            OnStateNotification?.Invoke(this, new HardwareEventArgs(message));
        }

        public void SetGridConnection(bool isConnected)
        {
            IsConnectedToGrid = isConnected;
            Notify(isConnected ? "Device plugged into grid." : "Grid power lost.");
        }

        public void ConnectPeripheral(IPeripheral peripheral)
        {
            ConnectedPeripherals.Add(peripheral);
            Notify($"{peripheral.Name} connected.");
        }

        public void ExecuteOperation(string softwareName, double durationHours, bool isIntensive)
        {
            bool hasAudio = ConnectedPeripherals.Any(p => p.IsAudioDevice);

            // Use SoftwareManager to check prerequisites
            if (!_softwareManager.ValidatePrerequisites(softwareName, DeviceMemory, HasNetworkConnection, hasAudio, out string error))
            {
                Notify($"Operation Failed: {error}");
                return;
            }

            // Use PowerService to calculate drains
            if (!_powerService.TryConsumePower(IsConnectedToGrid, DeviceBattery, DeviceUPS, durationHours, isIntensive, out string powerMsg))
            {
                Notify($"Power Failure: {powerMsg}");
                return;
            }

            Notify($"Operation '{softwareName}' completed successfully. {powerMsg}");
        }
    }
}