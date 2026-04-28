using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using HardwareSim.BLL.Entities.Components;
using HardwareSim.BLL.Entities.Base;
using HardwareSim.DAL.Interfaces;

namespace HardwareSim.DAL.Repositories
{
    public class CatalogRoot
    {
        public List<AppSoftware> SoftwareCatalog { get; set; } = new();
        public List<HardwarePeripheral> PeripheralCatalog { get; set; } = new();
    }

    public class JsonCatalogRepo : ICatalogRepository
    {
        private readonly string _filePath = "catalog.json";
        private CatalogRoot _catalog = new();
        private readonly JsonSerializerOptions _jsonOptions;

        public JsonCatalogRepo()
        {
            _jsonOptions = new JsonSerializerOptions { WriteIndented = true };
            LoadFromFile();
        }

        private void LoadFromFile()
        {
            if (File.Exists(_filePath))
            {
                string json = File.ReadAllText(_filePath);
                _catalog = JsonSerializer.Deserialize<CatalogRoot>(json, _jsonOptions) ?? new CatalogRoot();
            }
            else
            {
                SeedDefaultCatalog();
                SaveToFile();
            }
        }

        private void SaveToFile()
        {
            string json = JsonSerializer.Serialize(_catalog, _jsonOptions);
            File.WriteAllText(_filePath, json);
        }

        private void SeedDefaultCatalog()
        {
            _catalog.SoftwareCatalog.Add(new AppSoftware { Name = "Cyberpunk 2077", IsIntensive = true, RequiresAudio = true, RequiresNetwork = false, SupportedPlatform = DevicePlatform.Desktop });
            _catalog.SoftwareCatalog.Add(new AppSoftware { Name = "Spotify", IsIntensive = false, RequiresAudio = true, RequiresNetwork = true, SupportedPlatform = DevicePlatform.Universal });
            _catalog.SoftwareCatalog.Add(new AppSoftware { Name = "Instagram", IsIntensive = false, RequiresAudio = false, RequiresNetwork = true, SupportedPlatform = DevicePlatform.Mobile });
            _catalog.SoftwareCatalog.Add(new AppSoftware { Name = "Notepad", IsIntensive = false, RequiresAudio = false, RequiresNetwork = false, SupportedPlatform = DevicePlatform.Desktop });

            _catalog.PeripheralCatalog.Add(new HardwarePeripheral { Name = "Mechanical Keyboard", IsAudioDevice = false, SupportedPlatform = DevicePlatform.Desktop });
            _catalog.PeripheralCatalog.Add(new HardwarePeripheral { Name = "Earbuds", IsAudioDevice = true, SupportedPlatform = DevicePlatform.Universal });
            _catalog.PeripheralCatalog.Add(new HardwarePeripheral { Name = "Mouse", IsAudioDevice = false, SupportedPlatform = DevicePlatform.Desktop });
        }

        public IEnumerable<AppSoftware> GetAllSoftware() => _catalog.SoftwareCatalog;

        public IEnumerable<HardwarePeripheral> GetAllPeripherals() => _catalog.PeripheralCatalog;

        public void AddSoftware(AppSoftware software)
        {
            _catalog.SoftwareCatalog.Add(software);
        }

        public void AddPeripheral(HardwarePeripheral peripheral)
        {
            _catalog.PeripheralCatalog.Add(peripheral);
        }

        public void SaveChanges() => SaveToFile();
    }
}