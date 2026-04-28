using System;

namespace HardwareSim.BLL.Abstractions
{
    public interface IPeripheral
    {
        string Name { get; }
        bool IsAudioDevice { get; }
    }
}