using HardwareSim.BLL.Entities.Components;

namespace HardwareSim.BLL.Features.PowerManagement
{
    class IntensiveDrainStrategy : IPowerDrainStrategy
    {
        public double GetMultiplier(Battery activeSource)
        {
            return activeSource.DrainRateIntenseMultiplier;
        }
    }
}
