using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using BLL;
using Core.Entities;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL.Tests
{
    [TestClass]
    public class StudentServiceTests
    {
        private Mock<IStudentRepository> _mockRepo;
        private Mock<IInternetService> _mockNet;
        private StudentService _studentService;

        // [TestInitialize] - метод, що виконується перед *кожним* тестом
        [TestInitialize]
        public void TestSetup()
        {
            // Arrange (Загальна підготовка для всіх тестів)
            _mockRepo = new Mock<IStudentRepository>();
            _mockNet = new Mock<IInternetService>();

            _studentService = new StudentService(_mockRepo.Object, _mockNet.Object);
        }

        [TestMethod]
        public void GetSummerBornThirdYears_ReturnsOnlyCorrectStudents()
        {
            // Arrange (Підготовка)
            var students = new List<Student>
            {
                // Цей має підійти
                new Student { FirstName = "Літній", Course = 3, BirthDate = new DateTime(2000, 7, 15) },
                // Не той курс
                new Student { FirstName = "Не-третій", Course = 2, BirthDate = new DateTime(2001, 7, 20) },
                // Не той місяць
                new Student { FirstName = "Зимовий", Course = 3, BirthDate = new DateTime(2000, 1, 10) }
            };
            _mockRepo.Setup(r => r.GetStudents()).Returns(students);

            // Act (Дія)
            var result = _studentService.GetSummerBornThirdYears();

            // Assert (Перевірка)
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("Літній", result.First().FirstName);
        }

        [TestMethod]
        public void AdvanceStudentCourse_FromCourse5_IncrementsTo6()
        {
            var student = new Student { Course = 5 };

            _studentService.AdvanceStudentCourse(student);

            Assert.AreEqual(6, student.Course);
        }

        [TestMethod]
        public void AdvanceStudentCourse_FromCourse6_Stays6()
        {
            var student = new Student { Course = 6 };

            _studentService.AdvanceStudentCourse(student);

            Assert.AreEqual(6, student.Course);
        }

        [TestMethod]
        public void Sing_ReturnsCorrectString()
        {
            var student = new Student { FirstName = "Анна" };
            string expected = "Анна співає: Ла-ла-ла!";

            string result = _studentService.Sing(student);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void StudyOnline_WhenInternetIsConnected_ReturnsSuccess()
        {
            var student = new Student { FirstName = "Іван" };
            
            _mockNet.Setup(net => net.IsConnected()).Returns(true);
            string expected = "Іван вчиться онлайн.";

            string result = _studentService.StudyOnline(student);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void StudyOnline_WhenInternetIsOffline_ReturnsFailure()
        {
            var student = new Student { FirstName = "Іван" };
            
            _mockNet.Setup(net => net.IsConnected()).Returns(false);
            string expected = "Іван не може вчитись: немає інтернету.";

            string result = _studentService.StudyOnline(student);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void StudyOnline_Teacher_WhenInternetIsConnected_ReturnsSuccess()
        {
            var teacher = new Teacher { Name = "Марія" };
            _mockNet.Setup(net => net.IsConnected()).Returns(true);
            string expected = "Марія (вчитель) вчиться онлайн.";

            string result = _studentService.StudyOnline(teacher);

            Assert.AreEqual(expected, result);
        }
    }
}