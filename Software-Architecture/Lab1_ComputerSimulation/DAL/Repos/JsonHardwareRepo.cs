using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using HardwareSim.BLL.Entities.Base;
using HardwareSim.BLL.Entities.Devices;
using HardwareSim.BLL.Entities.Components;
using HardwareSim.DAL.Interfaces;

namespace HardwareSim.DAL.Repositories
{
    public class JsonHardwareRepo : IRepository
    {
        private readonly string _filePath = "devices.json";
        private Dictionary<string, Device> _deviceStorage = new();
        private readonly JsonSerializerOptions _jsonOptions;

        public JsonHardwareRepo()
        {
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            _jsonOptions.Converters.Add(new DeviceJsonConverter());

            LoadFromFile();
        }

        private void LoadFromFile()
        {
            if (File.Exists(_filePath))
            {
                string json = File.ReadAllText(_filePath);
                _deviceStorage = JsonSerializer.Deserialize<Dictionary<string, Device>>(json, _jsonOptions)
                                 ?? new Dictionary<string, Device>();
            }
            else
            {
                SeedInitialData();
            }
        }

        private void SaveToFile()
        {
            string json = JsonSerializer.Serialize(_deviceStorage, _jsonOptions);
            File.WriteAllText(_filePath, json);
        }

        private void SeedInitialData()
        {
            _deviceStorage.Add("DesktopPC", new Computer(new Processor("Intel i7", 3.5), includeUPS: true));
            _deviceStorage.Add("WorkLaptop", new Laptop(new Processor("AMD Ryzen 5", 2.8), 6000));
            _deviceStorage.Add("MyPhone", new Smartphone(new Processor("Snapdragon 8 Gen 2", 3.2), 2500));

            SaveToFile(); // Generate the file immediately
        }

        public IEnumerable<Device> GetAllDevices() => _deviceStorage.Values;

        public Device? GetDevice(string id) =>
            _deviceStorage.ContainsKey(id) ? _deviceStorage[id] : null;

        public void AddDevice(string id, Device device)
        {
            _deviceStorage[id] = device;
            SaveToFile(); // Save to JSON immediately when a new device is added
        }

        public void SaveChanges()
        {
            SaveToFile();
        }
    }
}