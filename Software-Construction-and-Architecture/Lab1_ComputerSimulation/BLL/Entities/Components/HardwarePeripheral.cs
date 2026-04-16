using HardwareSim.BLL.Abstractions;

namespace HardwareSim.BLL.Entities.Components
{
    public class HardwarePeripheral : IPeripheral
    {
        public string Name { get; set; } = null!;
        public bool IsAudioDevice { get; set; }
    }
}
