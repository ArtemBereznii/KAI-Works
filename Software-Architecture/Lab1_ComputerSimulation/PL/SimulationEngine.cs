using System;
using System.Linq;
using HardwareSim.BLL.Entities.Base;
using HardwareSim.BLL.Entities.Components;
using HardwareSim.DAL.Interfaces;

namespace HardwareSim.PL
{
    public class SimulationEngine
    {
        private readonly IRepository _repository;
        private Device? _currentDevice;

        public SimulationEngine(IRepository repository)
        {
            _repository = repository;
        }

        public void Start()
        {
            ConsoleManager.PrintInfo("Welcome to the Hardware Simulation.");
            SelectDevice();

            if (_currentDevice == null)
            {
                ConsoleManager.PrintInfo("No device selected. Exiting.");
                return;
            }

            bool running = true;
            while (running)
            {
                // Replace your existing string[] menuOptions and switch statement with this:
                string[] menuOptions = {
                    "Execute Operation (Work/Play/Music)",
                    "Toggle Power Grid Connection",
                    "Toggle Network Connection",
                    "Install Software",
                    "Uninstall Software", 
                    "Connect Peripheral",
                    "Disconnect Peripheral",
                    "Switch Device",
                    "Exit"
                };

                ConsoleManager.PrintMenu(menuOptions, $"{_currentDevice.GetType().Name} SIMULATION");

                string choice = Console.ReadLine() ?? "";

                switch (choice)
                {
                    case "1": RunOperation(); break;
                    case "2": TogglePower(); break;
                    case "3": ToggleNetwork(); break;
                    case "4": InstallSoftware(); break;
                    case "5": UninstallSoftware(); break;
                    case "6": ConnectPeripheral(); break;
                    case "7": DisconnectPeripheral(); break;
                    case "8": SelectDevice(); break;
                    case "9": running = false; break; 
                    default: ConsoleManager.PrintInfo("Invalid option."); break;
                }
            }
        }

        private void SelectDevice()
        {
            var devices = _repository.GetAllDevices().ToList();
            string[] deviceNames = devices.Select(d => d.GetType().Name).ToArray();

            ConsoleManager.PrintMenu(deviceNames, "SELECT A DEVICE TO USE");

            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= devices.Count)
            {
                // Unsubscribe from the old device to prevent duplicate messages
                if (_currentDevice != null)
                {
                    _currentDevice.OnStateNotification -= ConsoleManager.HandleDeviceNotification;
                }

                _currentDevice = devices[choice - 1];

                // Subscribe to the new device
                _currentDevice.OnStateNotification += ConsoleManager.HandleDeviceNotification;
                ConsoleManager.PrintInfo($"Switched to {_currentDevice.GetType().Name}.");
            }
            else
            {
                ConsoleManager.PrintInfo("Invalid selection.");
            }
        }

        private void TogglePower()
        {
            if (_currentDevice != null)
            {
                bool newState = !_currentDevice.IsConnectedToGrid;
                _currentDevice.SetGridConnection(newState);
            }
        }

        private void ToggleNetwork()
        {
            if (_currentDevice != null)
            {
                bool newState = !_currentDevice.HasNetworkConnection;
                _currentDevice.SetNetworkConnection(newState);

                _repository.SaveChanges();
            }
        }

        private void InstallSoftware()
        {
            string name = ConsoleManager.ReadInput("Enter software name to install");

            if (_currentDevice != null && _currentDevice.DeviceMemory.IsInstalled(name))
            {
                ConsoleManager.PrintInfo("Software is already installed.");
                return;
            }

            bool isIntensive = ConsoleManager.ReadBool("Is this intensive (e.g., Game/Video)?");
            bool reqAudio = ConsoleManager.ReadBool("Does it require audio?");
            bool reqNet = ConsoleManager.ReadBool("Does it require internet?");

            var newApp = new AppSoftware
            {
                Name = name,
                IsIntensive = isIntensive,
                RequiresAudio = reqAudio,
                RequiresNetwork = reqNet
            };

            _currentDevice?.DeviceMemory.Install(newApp);

            _repository.SaveChanges();

            ConsoleManager.PrintInfo($"{name} installed successfully.");
        }

        private void ConnectPeripheral()
        {
            string name = ConsoleManager.ReadInput("Enter peripheral name (e.g., 'Headphones', 'Mouse')");

            bool isAudio = ConsoleManager.ReadBool("Is this an audio device?");

            _currentDevice?.ConnectPeripheral(new HardwarePeripheral { Name = name, IsAudioDevice = isAudio });

            _repository.SaveChanges();

            ConsoleManager.PrintInfo($"{name} connected successfully.");
        }

        private void ListInstalledSoftware()
        {
            if (_currentDevice == null) return;

            var apps = _currentDevice.DeviceMemory.InstalledSoftware;

            if (!apps.Any())
            {
                ConsoleManager.PrintInfo("No software is currently installed on this device.");
                return;
            }

            ConsoleManager.PrintInfo("--- Installed Software ---");
            foreach (var app in apps)
            {
                string intensive = app.IsIntensive ? "Yes" : "No";
                string audio = app.RequiresAudio ? "Yes" : "No";
                string net = app.RequiresNetwork ? "Yes" : "No";

                ConsoleManager.PrintInfo($"- {app.Name} [Intensive: {intensive} | Audio: {audio} | Network: {net}]");
            }
            ConsoleManager.PrintInfo("--------------------------");
        }

        private void UninstallSoftware()
        {
            if (_currentDevice == null) return;

            ListInstalledSoftware();

            string name = ConsoleManager.ReadInput("Enter software name to uninstall");

            if (_currentDevice.DeviceMemory.Uninstall(name))
            {
                _repository.SaveChanges(); // Persist the deletion to the JSON file
                ConsoleManager.PrintInfo($"'{name}' was successfully uninstalled and changes were saved.");
            }
            else
            {
                ConsoleManager.PrintInfo($"Could not find '{name}' installed on this device.");
            }
        }

        private void DisconnectPeripheral()
        {
            if (_currentDevice == null) return;

            // Show the user what is currently connected
            if (!_currentDevice.ConnectedPeripherals.Any())
            {
                ConsoleManager.PrintInfo("No peripherals are currently connected.");
                return;
            }

            ConsoleManager.PrintInfo("Currently connected: " +
                string.Join(", ", _currentDevice.ConnectedPeripherals.Select(p => p.Name)));

            string name = ConsoleManager.ReadInput("Enter peripheral name to disconnect");

            _currentDevice.DisconnectPeripheral(name);
            _repository.SaveChanges(); // Persist the disconnection to the JSON file
        }

        private void RunOperation()
        {
            if (_currentDevice == null) return;

            string name = ConsoleManager.ReadInput("Enter software name to run");

            if (!_currentDevice.DeviceMemory.IsInstalled(name))
            {
                ConsoleManager.PrintInfo($"Operation Cancelled: '{name}' is not installed.");
                return;
            }

            if (!double.TryParse(ConsoleManager.ReadInput("Enter duration in hours (e.g., 2.5)"), out double hours))
            {
                ConsoleManager.PrintInfo("Invalid duration.");
                return;
            }

            _currentDevice.ExecuteOperation(name, hours);
        }

    }
}