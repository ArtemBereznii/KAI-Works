using HardwareSim.BLL.Entities.Components;
using System;

namespace HardwareSim.BLL.Services
{
    public class PowerService
    {
        public bool TryConsumePower(bool isConnectedToGrid, Battery? battery, Battery? ups, double durationHours, bool isIntensive, out string message)
        {
            // 1. Grid power is infinite.
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

            double multiplier = isIntensive ? activeSource.DrainRateIntenseMultiplier : 1.0;

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