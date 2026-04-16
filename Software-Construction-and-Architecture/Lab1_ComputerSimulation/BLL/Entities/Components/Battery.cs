using HardwareSim.BLL.Abstractions;

namespace HardwareSim.BLL.Entities.Components
{
    public class Battery : IPowerSource
    {
        public int CapacityMilliAmpHours { get; set;  }
        public double RemainingHours { get; set; }
        public bool IsEmpty => RemainingHours <= 0;

        public Battery() { }

        private readonly double _drainRateIntenseMultiplier;

        public Battery(int capacity)
        {
            CapacityMilliAmpHours = capacity;

            if (capacity >= 2000 && capacity <= 3000)
            {
                RemainingHours = 48.0;
                _drainRateIntenseMultiplier = 48.0 / 16.0;
            }
            else if (capacity >= 5000 && capacity <= 7000)
            {
                RemainingHours = 12.0;
                _drainRateIntenseMultiplier = 12.0 / 4.0;
            }
            else if (capacity == 500)
            {
                RemainingHours = 1;
                _drainRateIntenseMultiplier = 1.0;
            }
            else
            {
                RemainingHours = 0;
                _drainRateIntenseMultiplier = 1;
            }
        }

        public void Drain(double hours, bool isIntensive)
        {
            double drainAmount = isIntensive ? hours * _drainRateIntenseMultiplier : hours;
            RemainingHours -= drainAmount;
            if (RemainingHours < 0) RemainingHours = 0;
        }
    }
}