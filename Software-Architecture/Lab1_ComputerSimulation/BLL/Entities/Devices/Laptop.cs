using HardwareSim.BLL.Entities.Base;
using HardwareSim.BLL.Entities.Components;

namespace HardwareSim.BLL.Entities.Devices
{
    public class Laptop : Device
    {
        public override DevicePlatform Platform => DevicePlatform.Desktop;

        public Laptop() { }

        public Laptop(Processor processor, int batteryCapacity)
        {
            DeviceProcessor = processor;
            DeviceBattery = new Battery(batteryCapacity);
        }
    }
}