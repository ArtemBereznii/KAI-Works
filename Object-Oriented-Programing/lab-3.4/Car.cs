using System;

namespace Lab3_4
{
    public class Car(string model, double initialFuel, double consumptionRate = 10.0)
    {
        public string Model { get; } = model;
        public double FuelLevel { get; private set; } = initialFuel;
        public double FuelConsumptionRate { get; } = consumptionRate;

        public event EventHandler<OutOfGasEventArgs>? OutOfGas;

        /// Метод, що ініціює перевірку та виклик події.
        public void Drive(double distance)
        {
            double fuelNeeded = (distance / 100.0) * FuelConsumptionRate;
            Console.WriteLine($"\nСпроба проїхати {distance} км. Потрібно {fuelNeeded:F1} л палива.");

            if (fuelNeeded > FuelLevel)
            {
                double distancePossible = (FuelLevel / FuelConsumptionRate) * 100.0;
                double fuelDeficit = fuelNeeded - FuelLevel;
                FuelLevel = 0;

                Console.WriteLine($"Машина проїхала лише {distancePossible:F1} км і зупинилась.");

                OnOutOfGas(new OutOfGasEventArgs(fuelDeficit, DateTime.Now));
            }
            else
            {
                FuelLevel -= fuelNeeded;
                Console.WriteLine($"Машина успішно проїхала {distance} км. Залишок палива: {FuelLevel:F1} л.");
            }
        }

        public void Refuel(double amount)
        {
            FuelLevel += amount;
            Console.WriteLine($"Машину {Model} заправлено на {amount} л. Поточний рівень: {FuelLevel:F1} л.");
        }

        protected virtual void OnOutOfGas(OutOfGasEventArgs e)
        {
            OutOfGas?.Invoke(this, e);
        }
    }
}