using HardwareSim.BLL.Entities.Base;
using HardwareSim.BLL.Entities.Components;
using HardwareSim.BLL.Features.Store;
using HardwareSim.BLL.Features.SoftwareManagement;
using HardwareSim.BLL.Features.HardwareManagement;
using HardwareSim.BLL.Services;
using HardwareSim.DAL.Interfaces;

namespace HardwareSim.PL
{
    public class SimulationEngine
    {
        private readonly IRepository _repository;
        private readonly PowerService _powerService;
        private readonly StoreService _storeService;

        private Device? _currentDevice;

        public SimulationEngine(IRepository repository, PowerService powerService, StoreService storeService)
        {
            _repository = repository;
            _powerService = powerService;
            _storeService = storeService;
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
                string[] menuOptions = {
                    "Execute Operation (Work/Play/Music)",
                    "Toggle Power Grid Connection",
                    "Toggle Network Connection",
                    "Manage Software (App Store / Uninstall)",
                    "Manage Peripherals (Connect / Disconnect)",
                    "Switch Device",
                    "[Admin] Developer Store Portal",
                    "Exit"
                };

                ConsoleManager.PrintMenu(menuOptions, $"{_currentDevice.GetType().Name} SIMULATION");

                string choice = Console.ReadLine() ?? "";

                switch (choice)
                {
                    case "1": RunOperation(); break;
                    case "2": TogglePower(); break;
                    case "3": ToggleNetwork(); break;
                    case "4": ManageSoftware(); break;
                    case "5": ManagePeripherals(); break;
                    case "6": SelectDevice(); break;
                    case "7": OpenAdminMenu(); break;
                    case "8": running = false; break;
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
                if (_currentDevice != null)
                {
                    _currentDevice.OnStateNotification -= ConsoleManager.HandleDeviceNotification;
                }

                _currentDevice = devices[choice - 1];

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

        private void ManageSoftware()
        {
            if (_currentDevice == null) return;

            string[] options = { "Browse App Store (Install)", "Uninstall Software", "Back" };
            ConsoleManager.PrintMenu(options, "SOFTWARE MANAGEMENT");

            string choice = Console.ReadLine() ?? "";

            if (choice == "1") BrowseAppStore();
            else if (choice == "2") UninstallSoftware();
        }

        private void BrowseAppStore()
        {
            if (_currentDevice == null) return;

            Console.WriteLine($"\n--- WELCOME TO THE {_currentDevice.Platform.ToString().ToUpper()} APP STORE ---");
            var compatibleApps = _storeService.GetCompatibleSoftware(_currentDevice.Platform).ToList();

            for (int i = 0; i < compatibleApps.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {compatibleApps[i].Name} (Intensive: {compatibleApps[i].IsIntensive})");
            }

            Console.Write("Enter the number to install (or 0 to cancel): ");
            if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= compatibleApps.Count)
            {
                var selectedApp = compatibleApps[index - 1];

                if (_currentDevice.DeviceMemory.IsInstalled(selectedApp.Name))
                {
                    ConsoleManager.PrintInfo($"{selectedApp.Name} is already installed!");
                }
                else
                {
                    _currentDevice.DeviceMemory.Install(selectedApp);
                    _repository.SaveChanges();
                    ConsoleManager.PrintInfo($"Successfully installed {selectedApp.Name}!");
                }
            }
        }

        private void UninstallSoftware()
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
                ConsoleManager.PrintInfo($"- {app.Name}");
            }
            ConsoleManager.PrintInfo("--------------------------");

            string name = ConsoleManager.ReadInput("Enter software name to uninstall");

            if (_currentDevice.DeviceMemory.Uninstall(name))
            {
                _repository.SaveChanges();
                ConsoleManager.PrintInfo($"'{name}' was successfully uninstalled.");
            }
            else
            {
                ConsoleManager.PrintInfo($"Could not find '{name}' installed on this device.");
            }
        }

        private void ManagePeripherals()
        {
            if (_currentDevice == null) return;

            string[] options = { "Browse Hardware Store (Connect)", "Disconnect Peripheral", "Back" };
            ConsoleManager.PrintMenu(options, "PERIPHERAL MANAGEMENT");

            string choice = Console.ReadLine() ?? "";

            if (choice == "1") BrowseHardwareStore();
            else if (choice == "2") DisconnectPeripheral();
        }

        private void BrowseHardwareStore()
        {
            if (_currentDevice == null) return;

            Console.WriteLine($"\n--- WELCOME TO THE {_currentDevice.Platform.ToString().ToUpper()} HARDWARE STORE ---");
            var compatibleHardware = _storeService.GetCompatiblePeripherals(_currentDevice.Platform).ToList();

            for (int i = 0; i < compatibleHardware.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {compatibleHardware[i].Name}");
            }

            Console.Write("Enter the number to connect (or 0 to cancel): ");
            if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= compatibleHardware.Count)
            {
                _currentDevice.ConnectPeripheral(compatibleHardware[index - 1]);
                _repository.SaveChanges();
                ConsoleManager.PrintInfo($"Successfully connected {compatibleHardware[index - 1].Name}!");
            }
        }

        private void DisconnectPeripheral()
        {
            if (_currentDevice == null) return;

            if (!_currentDevice.ConnectedPeripherals.Any())
            {
                ConsoleManager.PrintInfo("No peripherals are currently connected.");
                return;
            }

            ConsoleManager.PrintInfo("Currently connected: " +
                string.Join(", ", _currentDevice.ConnectedPeripherals.Select(p => p.Name)));

            string name = ConsoleManager.ReadInput("Enter peripheral name to disconnect");

            _currentDevice.DisconnectPeripheral(name);
            _repository.SaveChanges();
        }

        private void OpenAdminMenu()
        {
            Console.WriteLine("\n--- 🔧 DEVELOPER ADMIN PORTAL 🔧 ---");
            Console.WriteLine("1. Publish New Software");
            Console.WriteLine("2. Publish New Peripheral");
            Console.Write("Select an option: ");
            string choice = Console.ReadLine() ?? "";

            try
            {
                if (choice == "1")
                {
                    string name = ConsoleManager.ReadInput("Enter App Name");
                    bool isIntensive = ConsoleManager.ReadBool("Is it intensive?");
                    bool reqAudio = ConsoleManager.ReadBool("Requires audio?");
                    bool reqNet = ConsoleManager.ReadBool("Requires network?");

                    Console.WriteLine("Select Platform (0=Desktop, 1=Mobile, 2=Universal): ");
                    DevicePlatform platform = (DevicePlatform)int.Parse(Console.ReadLine() ?? "2");

                    AppSoftware newApp = SoftwareFactory.CreateApp(name, isIntensive, reqAudio, reqNet, platform);

                    _storeService.PublishNewSoftware(newApp);
                    ConsoleManager.PrintInfo($"SUCCESS: '{name}' is now live in the global App Store!");
                }
                else if (choice == "2")
                {
                    string name = ConsoleManager.ReadInput("Enter Peripheral Name");
                    bool isAudio = ConsoleManager.ReadBool("Is it an audio device?");

                    Console.WriteLine("Select Platform (0=Desktop, 1=Mobile, 2=Universal): ");
                    DevicePlatform platform = (DevicePlatform)int.Parse(Console.ReadLine() ?? "2");

                    HardwarePeripheral newPeripheral = PeripheralFactory.CreatePeripheral(name, isAudio, platform);

                    _storeService.PublishNewPeripheral(newPeripheral);
                    ConsoleManager.PrintInfo($"SUCCESS: '{name}' is now available for purchase!");
                }
            }
            catch (Exception ex)
            {
                ConsoleManager.PrintInfo($"ADMIN ERROR: {ex.Message}");
            }
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