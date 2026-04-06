using System.Collections.Generic;
using HardwareSim.BLL.Entities.Base;
using HardwareSim.BLL.Entities.Devices;
using HardwareSim.BLL.Entities.Components;
using HardwareSim.DAL.Interfaces;

namespace HardwareSim.DAL.Repositories
{
    public class MockHardwareRepo : IRepository
    {
        private readonly Dictionary<string, Device> _deviceStorage = new();

        public MockHardwareRepo()
        {
            // Computer with a UPS
            _deviceStorage.Add("DesktopPC", new Computer(new Processor("Intel i7", 3.5), includeUPS: true));

            // Laptop with a 6000 mAh battery (5000-7000 range)
            _deviceStorage.Add("WorkLaptop", new Laptop(new Processor("AMD Ryzen 5", 2.8), 6000));

            // Smartphone with a 2500 mAh battery (2000-3000 range)
            _deviceStorage.Add("MyPhone", new Smartphone(new Processor("Snapdragon 8 Gen 2", 3.2), 2500));
        }

        public IEnumerable<Device> GetAllDevices() => _deviceStorage.Values;

        public Device GetDevice(string id) =>
            _deviceStorage.ContainsKey(id) ? _deviceStorage[id] : null;
    }
}