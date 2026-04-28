using System;

namespace HardwareSim.BLL.Abstractions
{
    public interface IPowerSource
    {
        double RemainingHours { get; }
        bool IsEmpty { get; }
        void Drain(double hours, bool isIntensive);
    }
}