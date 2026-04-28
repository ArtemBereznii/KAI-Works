using HardwareSim.BLL.Abstractions;

namespace HardwareSim.BLL.Entities.Components
{
    public class Battery : IPowerSource
    {
        public int CapacityMilliAmpHours { get; set;  }
        public double MaxRemainingHours { get; set; }
        public double RemainingHours { get; set; }
        public double DrainRateIntenseMultiplier { get; set; }
        public bool IsEmpty => RemainingHours <= 0;

        public Battery() { }

        public Battery(int capacity)
        {
            CapacityMilliAmpHours = capacity;

            if (capacity >= 2000 && capacity <= 3000)
            {
                MaxRemainingHours = 48.0;
                RemainingHours = MaxRemainingHours;
                DrainRateIntenseMultiplier = 48.0 / 16.0;
            }
            else if (capacity >= 5000 && capacity <= 7000)
            {
                MaxRemainingHours = 12.0;
                RemainingHours = MaxRemainingHours;
                DrainRateIntenseMultiplier = 12.0 / 4.0;
            }
            else if (capacity == 500)
            {
                MaxRemainingHours = 0.5;
                RemainingHours = MaxRemainingHours;
                DrainRateIntenseMultiplier = 1.0;
            }
            else
            {
                MaxRemainingHours = 0;
                RemainingHours = 0;
                DrainRateIntenseMultiplier = 1;
            }
        }

        public void Drain(double hours, bool isIntensive)
        {
            double drainAmount = isIntensive ? hours * DrainRateIntenseMultiplier : hours;
            RemainingHours -= drainAmount;
            if (RemainingHours < 0) RemainingHours = 0;
        }

        public void Recharge()
        {
            RemainingHours = MaxRemainingHours;
        }
    }
}