using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LabOOP3
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            //Створення початкових даних
            var student1 = new Student("CD789012", "Іваненко Іван Іванович", 2004, "ІП-32", 2);
            var student2 = new Student("AB123456", "Петренко Петро Петрович", 2005, "ІП-31");
            var student3 = new Student("EF345678", "Сидоренко Сидір Сидорович", 2006, "ІП-31");

            //Демонстрація роботи з колекціями
            DemonstrateCollections(student1, student2, student3);

            //Демонстрація роботи з бінарним деревом
            DemonstrateBinaryTree(student1, student2, student3);
        }

        public static void DemonstrateCollections(Student st1, Student st2, Student st3)
        {
            Console.WriteLine("\n========== РОБОТА З КОЛЕКЦІЯМИ ==========");

            // 1. Узагальнена колекція List<T> (рекомендований підхід)
            Console.WriteLine("\n--- 1. Демонстрація List<Student> ---");
            List<Student> studentList = new List<Student> { st1, st2, st3 };
            Console.WriteLine("Початковий список:");
            studentList.ForEach(s => s.DisplayInfo());

            Console.WriteLine("\n-> Оновлення: змінюємо групу для Петренка П.П.");
            var studentToUpdate = studentList.Find(s => s.TicketNumber == "AB123456");
            if (studentToUpdate != null) studentToUpdate.GroupNumber = "ІП-33";

            Console.WriteLine("-> Видалення: видаляємо Іваненка І.І.");
            studentList.Remove(st1);

            Console.WriteLine("\nКінцевий список (List<Student>):");
            studentList.ForEach(s => s.DisplayInfo());

            // 2. Масив Student[]
            Console.WriteLine("\n--- 2. Демонстрація масиву Student[] ---");
            Student[] studentArray = { st1, st2, st3 };
            Console.WriteLine("Початковий масив:");
            Array.ForEach(studentArray, s => s.DisplayInfo());

            Console.WriteLine("\n-> Видалення: видаляємо Сидоренка С.С.");
            studentArray = studentArray.Where(s => s.TicketNumber != "EF345678").ToArray();
            Console.WriteLine("Кінцевий масив:");
            Array.ForEach(studentArray, s => s.DisplayInfo());

            // 3. Неузагальнена колекція ArrayList (застарілий підхід)
            Console.WriteLine("\n--- 3. Демонстрація ArrayList ---");
            ArrayList arrayList = new ArrayList { st1, st2, st3 };
            Console.WriteLine("Початковий ArrayList:");
            foreach (Student s in arrayList) s.DisplayInfo();

            Console.WriteLine("\n-> Додавання об'єкта іншого типу (можливо в ArrayList):");
            arrayList.Add("не студент");

            Console.WriteLine("-> Видалення: видаляємо Петренка П.П.");
            arrayList.Remove(st2);

            Console.WriteLine("\nКінцевий ArrayList (з перевіркою типу):");
            foreach (object obj in arrayList)
            {
                if (obj is Student student)
                {
                    student.DisplayInfo();
                }
                else
                {
                    Console.WriteLine($"Знайдено об'єкт іншого типу: {obj}");
                }
            }
        }

        public static void DemonstrateBinaryTree(Student st1, Student st2, Student st3)
        {
            Console.WriteLine("\n\n====== ЗАВДАННЯ 3-4: РОБОТА З БІНАРНИМ ДЕРЕВОМ ======");

            var studentTree = new BinaryTree<Student>();

            // Додавання елементів
            studentTree.Add(st1);
            studentTree.Add(st2);
            studentTree.Add(st3);

            Console.WriteLine("\nСтудентів було додано до бінарного дерева пошуку.");
            Console.WriteLine("Дерево автоматично відсортувало їх за номером квитка.");

            Console.WriteLine("\n-> Обхід дерева у прямому порядку (pre-order) за допомогою ітератора (foreach):");
            foreach (var student in studentTree)
            {
                student.DisplayInfo();
            }
        }
    }
}