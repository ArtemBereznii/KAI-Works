using System;

namespace HardwareSim.BLL.Entities.Components
{
    public class Processor
    {
        public string ModelName { get; set; }
        public double ClockSpeedGHz { get; set; }

        public Processor(string model, double clockSpeed)
        {
            ModelName = model;
            ClockSpeedGHz = clockSpeed;
        }
    }
}