using System.Collections.Generic;
using HardwareSim.BLL.Entities.Base;

namespace HardwareSim.DAL.Interfaces
{
    public interface IRepository
    {
        IEnumerable<Device> GetAllDevices();
        Device? GetDevice(string id);
        void AddDevice(string id, Device device);

        void SaveChanges();
    }
}