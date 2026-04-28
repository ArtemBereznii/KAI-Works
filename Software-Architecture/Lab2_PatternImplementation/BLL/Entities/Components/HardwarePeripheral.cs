using HardwareSim.BLL.Abstractions;
using HardwareSim.BLL.Entities.Base;
namespace HardwareSim.BLL.Entities.Components
{
    public class HardwarePeripheral : IPeripheral
    {
        public string Name { get; set; } = string.Empty;
        public bool IsAudioDevice { get; set; }

        // NEW: The Hardware Store will use this to filter compatibility
        public DevicePlatform SupportedPlatform { get; set; } = DevicePlatform.Universal;

        public HardwarePeripheral() { }
    }
}