using HardwareSim.BLL.Entities.Components;

namespace HardwareSim.BLL.Features.PowerManagement
{
    public class NormalDrainStrategy : IPowerDrainStrategy
    {
        public double GetMultiplier(Battery activeSource)
        {
            return 1.0;
        }
    }
}
