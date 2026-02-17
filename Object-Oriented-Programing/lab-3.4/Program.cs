using System;
using System.Linq;

namespace Lab3_4
{
    class Program
    {
        // 1. Оголошуємо тип делегата з відповідним підписом 
        // ("Сортування одновимірного масиву цілих чисел за зростанням" )
        public delegate void SortDelegate(int[] array);

        // Метод для демонстрації
        public static void PrintArray(string title, int[] array)
        {
            Console.WriteLine($"{title}: [ {string.Join(", ", array)} ]");
        }

        public static void DemonstrateTask1()
        {
            Console.WriteLine("========= ЗАВДАННЯ 1: ДЕЛЕГАТИ ТА ЛЯМБДИ =========");
            int[] numbers = { 5, 1, 9, 3, 7, 2, 8 };
            PrintArray("Початковий масив", numbers);

            // 2. Використання Анонімного методу
            SortDelegate anonymousSorter = delegate (int[] arr)
            {
                Array.Sort(arr);
            };
            anonymousSorter(numbers);
            PrintArray("Після анонімного методу", numbers);

            numbers = new int[] { 5, 1, 9, 3, 7, 2, 8 };

            // 3. Використання Лямбда-виразу 
            SortDelegate lambdaSorter = (int[] arr) =>
            {
                Array.Sort(arr);
            };
            lambdaSorter(numbers);
            PrintArray("Після лямбда-виразу", numbers);
        }

        // ------------------ ЗАВДАННЯ 2 і 3 ------------------

        public static void DemonstrateTask2And3()
        {
            Console.WriteLine("\n\n========= ЗАВДАННЯ 2-3: ПОДІЇ =========");

            // 1. Створюємо компонент (видавця) 
            Car myCar = new Car("BMW", 10.0, 10.0);

            // 2. Створюємо підписника з обробником 
            Driver driver = new Driver("Олександр");

            // 3. Підписуємо обробник на подію 
            myCar.OutOfGas += driver.OnCarOutOfGas;

            // 4. Демонстрація
            myCar.Drive(50);
            myCar.Drive(70);
            myCar.Refuel(20.0);
            myCar.Drive(100);
        }

        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            DemonstrateTask1();
            DemonstrateTask2And3();
        }
    }
}