using System;
using HardwareSim.BLL.Abstractions;

namespace HardwareSim.BLL.Services
{
    public class PowerService
    {
        // Added '?' to IPowerSource to allow nulls (e.g., when a Computer has no battery)
        public bool TryConsumePower(bool hasGrid, IPowerSource? battery, IPowerSource? ups, double hours, bool isIntensive, out string notification)
        {
            if (hasGrid)
            {
                notification = "Powered by electrical grid.";
                return true;
            }

            if (ups != null && !ups.IsEmpty)
            {
                if (ups.RemainingHours >= hours)
                {
                    ups.Drain(hours, isIntensive);
                    notification = $"Powered by UPS. Remaining UPS hours: {Math.Round(ups.RemainingHours, 2)}";
                    return true;
                }
                ups.Drain(ups.RemainingHours, false); // Drain rest
                notification = "UPS depleted. Device shutting down.";
                return false;
            }

            if (battery != null && !battery.IsEmpty)
            {
                battery.Drain(hours, isIntensive);
                if (battery.IsEmpty)
                {
                    notification = "Battery died during operation. Device shutting down.";
                    return false;
                }
                notification = $"Powered by Battery. Remaining non-intense hours: {Math.Round(battery.RemainingHours, 2)}";
                return true;
            }

            notification = "No power source available. Device is off.";
            return false;
        }
    }
}