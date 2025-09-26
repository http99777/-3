using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Student
{
    public string LastName { get; set; }
    public int Course { get; set; }
    public string Group { get; set; }
    public string Institute { get; set; }
    public List<int> Grades { get; set; }

    public Student(string lastName, int course, string group, string institute, List<int> grades)
    {
        LastName = lastName;
        Course = course;
        Group = group;
        Institute = institute;
        Grades = grades;
    }

    public bool IsExcellentStudent => Grades.All(grade => grade == 5);
    public double AverageGrade => Grades.Count > 0 ? Grades.Average() : 0;
}

class Program
{
    static List<Student> students = new List<Student>();

    static void Main(string[] args)
    {
        students.Add(new Student("Иванов", 1, "ИТ-101", "ИТИ", new List<int> { 5, 5, 5, 5 }));
        students.Add(new Student("Петров", 1, "ИТ-101", "ИТИ", new List<int> { 5, 4, 5, 4 }));
        students.Add(new Student("Сидоров", 2, "ИТ-201", "ИТИ", new List<int> { 3, 3, 4, 5 }));
        students.Add(new Student("Кузнецов", 1, "ЭК-101", "ЭКИ", new List<int> { 5, 5, 4, 5 }));
        students.Add(new Student("Смирнов", 2, "МШ-201", "МШИ", new List<int> { 5, 5, 5, 5 }));

        while (true)
        {
            Console.Clear();
            Console.WriteLine("1 - Выполнить запрос");
            Console.WriteLine("2 - Показать всех студентов");
            Console.WriteLine("3 - Сохранить в файл");
            Console.WriteLine("4 - Выход");
            Console.Write("Выберите: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1": Query15(); break;
                case "2": ShowStudents(); break;
                case "3": SaveToFile(); break;
                case "4": return;
            }

            Console.WriteLine("\nНажмите Enter...");
            Console.ReadLine();
        }
    }

    static void Query15()
    {
        Console.WriteLine("\n=== Отличники на 1-2 курсах ===");

        var excellentStudents = students
            .Where(s => s.IsExcellentStudent && (s.Course == 1 || s.Course == 2))
            .ToList();

        foreach (var student in excellentStudents)
        {
            Console.WriteLine($"{student.LastName} - {student.Institute}, {student.Group}, курс {student.Course}");
        }

        Console.WriteLine("\n=== Средний балл по группам (1-2 курсы) ===");

        var groupAverages = students
            .Where(s => s.Course == 1 || s.Course == 2)
            .GroupBy(s => new { s.Institute, s.Group, s.Course })
            .Select(g => new {
                Institute = g.Key.Institute,
                Group = g.Key.Group,
                Course = g.Key.Course,
                Average = g.Average(s => s.AverageGrade)
            })
            .OrderByDescending(x => x.Average)
            .ToList();

        foreach (var group in groupAverages)
        {
            Console.WriteLine($"{group.Institute}, {group.Group}, курс {group.Course}: {group.Average:F2}");
        }
    }

    static void ShowStudents()
    {
        Console.WriteLine("\n=== Все студенты ===");
        foreach (var student in students)
        {
            Console.WriteLine($"{student.LastName} - {student.Institute}, {student.Group}, курс {student.Course}, ср.балл: {student.AverageGrade:F2}");
        }
    }

    static void SaveToFile()
    {
        string filename = "result.txt";

        using (StreamWriter writer = new StreamWriter(filename))
        {
            writer.WriteLine("Результаты запроса 15");
            writer.WriteLine("Отличники на 1-2 курсах:");

            var excellentStudents = students
                .Where(s => s.IsExcellentStudent && (s.Course == 1 || s.Course == 2))
                .ToList();

            foreach (var student in excellentStudents)
            {
                writer.WriteLine($"{student.LastName} - {student.Institute}, {student.Group}, курс {student.Course}");
            }

            writer.WriteLine("\nСредний балл по группам:");

            var groupAverages = students
                .Where(s => s.Course == 1 || s.Course == 2)
                .GroupBy(s => new { s.Institute, s.Group, s.Course })
                .Select(g => new {
                    Institute = g.Key.Institute,
                    Group = g.Key.Group,
                    Course = g.Key.Course,
                    Average = g.Average(s => s.AverageGrade)
                })
                .OrderByDescending(x => x.Average);

            foreach (var group in groupAverages)
            {
                writer.WriteLine($"{group.Institute}, {group.Group}, курс {group.Course}: {group.Average:F2}");
            }
        }

        Console.WriteLine($"Результаты сохранены в файл: {filename}");
    }
}