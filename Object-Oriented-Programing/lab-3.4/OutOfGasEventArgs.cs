using System;

namespace Lab3_4
{
    
    /// Клас-аргумент, що містить додаткову інформацію про подію
    public class OutOfGasEventArgs : EventArgs
    {
        public double FuelDeficit { get; }

        public DateTime TimeOfEvent { get; }

        public OutOfGasEventArgs(double deficit, DateTime time)
        {
            FuelDeficit = deficit;
            TimeOfEvent = time;
        }
    }
}