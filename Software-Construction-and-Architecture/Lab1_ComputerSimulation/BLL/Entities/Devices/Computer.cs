using HardwareSim.BLL.Entities.Base;
using HardwareSim.BLL.Entities.Components;

namespace HardwareSim.BLL.Entities.Devices
{
    public class Computer : Device
    {
        public Computer(Processor processor, bool includeUPS = false)
        {
            DeviceProcessor = processor;

            if (includeUPS)
            {
                // A UPS acting as a power source with 0.5 hours (30 mins) capacity
                DeviceUPS = new Battery(500);
                // Note: In a real app, UPS would have its own class implementing IPowerSource
            }
            // Computers intentionally do not get a DeviceBattery assigned [cite: 58]
        }
    }
}