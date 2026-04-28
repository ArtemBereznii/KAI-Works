using HardwareSim.BLL.Entities.Components;
using HardwareSim.BLL.Features.PowerManagement;

namespace HardwareSim.BLL.Services
{
    public class PowerService
    {
        // 4. КОНТЕКСТ (The Context using the Strategy)
        public bool TryConsumePower(bool isConnectedToGrid, Battery? battery, Battery? ups, double durationHours, IPowerDrainStrategy drainStrategy, out string message)
        {
            if (isConnectedToGrid)
            {
                message = "Powered by the electrical grid. No backup power consumed.";
                return true;
            }

            Battery? activeSource = battery ?? ups;

            if (activeSource == null)
            {
                message = "POWER FAILURE! This device has no battery or UPS connected.";
                return false;
            }

            string sourceName = battery != null ? "Battery" : "UPS";

            double multiplier = drainStrategy.GetMultiplier(activeSource);

            double requiredCapacity = durationHours * multiplier;
            double availableCapacity = activeSource.RemainingHours;

            if (availableCapacity >= requiredCapacity)
            {
                activeSource.RemainingHours -= requiredCapacity;
                message = $"Completed on {sourceName}. Remaining time: {activeSource.RemainingHours:F2} hours.";
                return true;
            }
            else
            {
                double capacityShortfall = requiredCapacity - availableCapacity;
                double actualHoursShort = capacityShortfall / multiplier;
                activeSource.RemainingHours = 0;

                message = $"POWER FAILURE! The {sourceName} died during the operation. You were short by {actualHoursShort:F2} hours.";
                return false;
            }
        }
    }
}