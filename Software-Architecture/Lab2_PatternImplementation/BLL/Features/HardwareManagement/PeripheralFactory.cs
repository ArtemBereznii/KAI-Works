using HardwareSim.BLL.Entities.Base;
using HardwareSim.BLL.Entities.Components;

namespace HardwareSim.BLL.Features.HardwareManagement
{
    public static class PeripheralFactory
    {
        public static HardwarePeripheral CreatePeripheral(string name, bool isAudio, DevicePlatform platform)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Peripheral name cannot be empty.");
            }

            return new HardwarePeripheral
            {
                Name = name,
                IsAudioDevice = isAudio,
                SupportedPlatform = platform
            };
        }
    }
}