using HardwareSim.BLL.Abstractions;

namespace HardwareSim.BLL.Entities.Components
{
    public class HardwarePeripheral : IPeripheral
    {
        public string Name { get; set; }
        public bool IsAudioDevice { get; set; }
    }
}