using HardwareSim.BLL.Entities.Base;
using HardwareSim.BLL.Entities.Components;
using HardwareSim.DAL.Interfaces;
using HardwareSim.PL;

namespace HardwareSim.PL
{
    public class DemoRunner
    {
        private readonly IRepository _repository;

        public DemoRunner(IRepository repository)
        {
            _repository = repository;
        }

        public void RunDemo()
        {
            ConsoleManager.PrintHeader("Starting Hardware Simulation Demo");

            Device laptop = _repository.GetDevice("WorkLaptop");

            // 1. Subscribe to events (Crucial for points!) 
            laptop.OnStateNotification += ConsoleManager.HandleDeviceNotification;

            ConsoleManager.PrintHeader("Scenario 1: Missing Prerequisites");

            // Fails: Software not installed
            laptop.ExecuteOperation("WordProcessor", 1.0, isIntensive: false);

            ConsoleManager.PrintHeader("Scenario 2: Fulfilling Prerequisites");
            laptop.DeviceMemory.Install("OnlineGame");
            laptop.HasNetworkConnection = false;

            // Fails: No network
            laptop.ExecuteOperation("OnlineGame", 2.0, isIntensive: true);

            // Fix network
            laptop.HasNetworkConnection = true;

            // Fails: No audio device for game
            laptop.ExecuteOperation("OnlineGame", 2.0, isIntensive: true);

            // Connect audio
            laptop.ConnectPeripheral(new HardwarePeripheral { Name = "Gaming Headset", IsAudioDevice = true });

            ConsoleManager.PrintHeader("Scenario 3: Working on Grid Power");

            // Succeeds: All prerequisites met, connected to grid
            laptop.ExecuteOperation("OnlineGame", 4.0, isIntensive: true);

            ConsoleManager.PrintHeader("Scenario 4: Power Outage & Battery Drain");
            laptop.SetGridConnection(false);

            // Succeeds: Runs on battery
            laptop.ExecuteOperation("OnlineGame", 3.0, isIntensive: true);

            // Over-drain: Should kill the battery (6000 mAh = 4 hours intense max)
            laptop.ExecuteOperation("OnlineGame", 2.0, isIntensive: true);

            // Clean up subscription
            laptop.OnStateNotification -= ConsoleManager.HandleDeviceNotification;
        }
    }
}