using System.Collections.Generic;
using System.Linq;
using HardwareSim.BLL.Entities.Base;
using HardwareSim.BLL.Entities.Components;
using HardwareSim.DAL.Interfaces;

namespace HardwareSim.BLL.Services
{
    public class StoreService
    {
        private readonly ICatalogRepository _catalogRepo;

        public StoreService(ICatalogRepository catalogRepo)
        {
            _catalogRepo = catalogRepo;
        }

        public IEnumerable<AppSoftware> GetCompatibleSoftware(DevicePlatform devicePlatform)
        {
            return _catalogRepo.GetAllSoftware()
                .Where(app => app.SupportedPlatform == DevicePlatform.Universal ||
                              app.SupportedPlatform == devicePlatform);
        }

        public IEnumerable<HardwarePeripheral> GetCompatiblePeripherals(DevicePlatform devicePlatform)
        {
            return _catalogRepo.GetAllPeripherals()
                .Where(p => p.SupportedPlatform == DevicePlatform.Universal ||
                            p.SupportedPlatform == devicePlatform);
        }

        public void PublishNewSoftware(AppSoftware newApp)
        {
            if (_catalogRepo.GetAllSoftware().Any(app => app.Name.ToLower() == newApp.Name.ToLower()))
            {
                throw new System.Exception($"Software '{newApp.Name}' already exists in the store!");
            }

            _catalogRepo.AddSoftware(newApp);
            _catalogRepo.SaveChanges();
        }

        public void PublishNewPeripheral(HardwarePeripheral newPeripheral)
        {
            if (_catalogRepo.GetAllPeripherals().Any(p => p.Name.ToLower() == newPeripheral.Name.ToLower()))
            {
                throw new System.Exception($"Peripheral '{newPeripheral.Name}' already exists in the store!");
            }

            _catalogRepo.AddPeripheral(newPeripheral);
            _catalogRepo.SaveChanges();
        }
    }
}