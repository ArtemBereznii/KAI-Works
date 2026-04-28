using HardwareSim.BLL.Entities.Components;

namespace HardwareSim.BLL.Features.PowerManagement
{
    public interface IPowerDrainStrategy
    {
        double GetMultiplier(Battery activeSource);
    }
}
