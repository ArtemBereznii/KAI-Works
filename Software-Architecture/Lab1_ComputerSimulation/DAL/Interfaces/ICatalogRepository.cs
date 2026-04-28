using System.Collections.Generic;
using HardwareSim.BLL.Entities.Components;

namespace HardwareSim.DAL.Interfaces
{
    public interface ICatalogRepository
    {
        IEnumerable<AppSoftware> GetAllSoftware();
        IEnumerable<HardwarePeripheral> GetAllPeripherals();

        void AddSoftware(AppSoftware software);
        void AddPeripheral(HardwarePeripheral peripheral);

        void SaveChanges();
    }
}