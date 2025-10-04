using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class LastName
{
    public string Value { get; }

    public LastName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Фамилия не может быть пустой");
        Value = value;
    }

    public override string ToString() => Value;
    public override bool Equals(object obj) => obj is LastName other && Value == other.Value;
    public override int GetHashCode() => Value.GetHashCode();
}

public class Course
{
    public int Value { get; }

    public Course(int value)
    {
        if (value < 1 || value > 6)
            throw new ArgumentException("Курс должен быть от 1 до 6");
        Value = value;
    }

    public override string ToString() => Value.ToString();
    public override bool Equals(object obj) => obj is Course other && Value == other.Value;
    public override int GetHashCode() => Value.GetHashCode();
}

public class Group
{
    public string Value { get; }

    public Group(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Группа не может быть пустой");
        Value = value;
    }

    public override string ToString() => Value;
    public override bool Equals(object obj) => obj is Group other && Value == other.Value;
    public override int GetHashCode() => Value.GetHashCode();
}

public class Institute
{
    public string Value { get; }

    public Institute(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Институт не может быть пустым");
        Value = value;
    }

    public override string ToString() => Value;
    public override bool Equals(object obj) => obj is Institute other && Value == other.Value;
    public override int GetHashCode() => Value.GetHashCode();
}

public class Student
{
    public LastName StudentLastName { get; set; }
    public Course StudentCourse { get; set; }
    public Group StudentGroup { get; set; }
    public Institute StudentInstitute { get; set; }
    public List<int> Grades { get; set; }

    public Student(LastName lastName, Course course, Group group, Institute institute, List<int> grades)
    {
        StudentLastName = lastName;
        StudentCourse = course;
        StudentGroup = group;
        StudentInstitute = institute;
        Grades = grades;
    }
}

public class StudentAnalyzer
{
    private Student _student;

    public StudentAnalyzer(Student student)
    {
        _student = student;
    }

    public bool IsExcellentStudent => _student.Grades.All(grade => grade == 5);
    public double AverageGrade => _student.Grades.Count > 0 ? _student.Grades.Average() : 0;
}

public class StudentQueryService
{
    private List<Student> _students;

    public StudentQueryService(List<Student> students)
    {
        _students = students;
    }

    public List<Student> GetExcellentStudentsOnFirstSecondCourses()
    {
        return _students
            .Where(s => new StudentAnalyzer(s).IsExcellentStudent && 
                       (s.StudentCourse.Value == 1 || s.StudentCourse.Value == 2))
            .ToList();
    }

    public List<GroupAverageResult> GetGroupAveragesForFirstSecondCourses()
    {
        return _students
            .Where(s => s.StudentCourse.Value == 1 || s.StudentCourse.Value == 2)
            .GroupBy(s => new { 
                Institute = s.StudentInstitute, 
                Group = s.StudentGroup, 
                Course = s.StudentCourse 
            })
            .Select(g => new GroupAverageResult
            {
                Institute = g.Key.Institute,
                Group = g.Key.Group,
                Course = g.Key.Course,
                Average = g.Average(s => new StudentAnalyzer(s).AverageGrade)
            })
            .OrderByDescending(x => x.Average)
            .ToList();
    }

    public List<Student> GetAllStudents()
    {
        return _students;
    }
}


public class GroupAverageResult
{
    public Institute Institute { get; set; }
    public Group Group { get; set; }
    public Course Course { get; set; }
    public double Average { get; set; }
}

public class FileExporter
{
    private StudentQueryService _queryService;

    public FileExporter(StudentQueryService queryService)
    {
        _queryService = queryService;
    }

    public void SaveToFile(string filename = "result.txt")
    {
        using (StreamWriter writer = new StreamWriter(filename))
        {
            writer.WriteLine("Результаты запроса 15");
            writer.WriteLine("Отличники на 1-2 курсах:");

            var excellentStudents = _queryService.GetExcellentStudentsOnFirstSecondCourses();
            foreach (var student in excellentStudents)
            {
                writer.WriteLine($"{student.StudentLastName} - {student.StudentInstitute}, {student.StudentGroup}, курс {student.StudentCourse}");
            }

            writer.WriteLine("\nСредний балл по группам:");

            var groupAverages = _queryService.GetGroupAveragesForFirstSecondCourses();
            foreach (var group in groupAverages)
            {
                writer.WriteLine($"{group.Institute}, {group.Group}, курс {group.Course}: {group.Average:F2}");
            }
        }
    }
}

public class ConsoleDisplayService
{
    private StudentQueryService _queryService;

    public ConsoleDisplayService(StudentQueryService queryService)
    {
        _queryService = queryService;
    }

    public void ShowExcellentStudentsOnFirstSecondCourses()
    {
        Console.WriteLine("\n=== Отличники на 1-2 курсах ===");

        var excellentStudents = _queryService.GetExcellentStudentsOnFirstSecondCourses();
        foreach (var student in excellentStudents)
        {
            Console.WriteLine($"{student.StudentLastName} - {student.StudentInstitute}, {student.StudentGroup}, курс {student.StudentCourse}");
        }
    }

    public void ShowGroupAveragesForFirstSecondCourses()
    {
        Console.WriteLine("\n=== Средний балл по группам (1-2 курсы) ===");

        var groupAverages = _queryService.GetGroupAveragesForFirstSecondCourses();
        foreach (var group in groupAverages)
        {
            Console.WriteLine($"{group.Institute}, {group.Group}, курс {group.Course}: {group.Average:F2}");
        }
    }

    public void ShowAllStudents()
    {
        Console.WriteLine("\n=== Все студенты ===");
        var allStudents = _queryService.GetAllStudents();
        foreach (var student in allStudents)
        {
            var analyzer = new StudentAnalyzer(student);
            Console.WriteLine($"{student.StudentLastName} - {student.StudentInstitute}, {student.StudentGroup}, курс {student.StudentCourse}, ср.балл: {analyzer.AverageGrade:F2}");
        }
    }
}

public static class StudentFactory
{
    public static LastName CreateLastName(string value) => new LastName(value);
    public static Course CreateCourse(int value) => new Course(value);
    public static Group CreateGroup(string value) => new Group(value);
    public static Institute CreateInstitute(string value) => new Institute(value);
}

class Program
{
    private static List<Student> students = new List<Student>();
    private static StudentQueryService queryService;
    private static ConsoleDisplayService displayService;
    private static FileExporter fileExporter;

    static void InitializeData()
    {
        students.Add(new Student(
            StudentFactory.CreateLastName("Иванов"),
            StudentFactory.CreateCourse(1),
            StudentFactory.CreateGroup("ИТ-101"),
            StudentFactory.CreateInstitute("ИТИ"),
            new List<int> { 5, 5, 5, 5 }
        ));
        
        students.Add(new Student(
            StudentFactory.CreateLastName("Петров"),
            StudentFactory.CreateCourse(1),
            StudentFactory.CreateGroup("ИТ-101"),
            StudentFactory.CreateInstitute("ИТИ"),
            new List<int> { 5, 4, 5, 4 }
        ));
        
        students.Add(new Student(
            StudentFactory.CreateLastName("Сидоров"),
            StudentFactory.CreateCourse(2),
            StudentFactory.CreateGroup("ИТ-201"),
            StudentFactory.CreateInstitute("ИТИ"),
            new List<int> { 3, 3, 4, 5 }
        ));
        
        students.Add(new Student(
            StudentFactory.CreateLastName("Кузнецов"),
            StudentFactory.CreateCourse(1),
            StudentFactory.CreateGroup("ЭК-101"),
            StudentFactory.CreateInstitute("ЭКИ"),
            new List<int> { 5, 5, 4, 5 }
        ));
        
        students.Add(new Student(
            StudentFactory.CreateLastName("Смирнов"),
            StudentFactory.CreateCourse(2),
            StudentFactory.CreateGroup("МШ-201"),
            StudentFactory.CreateInstitute("МШИ"),
            new List<int> { 5, 5, 5, 5 }
        ));

        queryService = new StudentQueryService(students);
        displayService = new ConsoleDisplayService(queryService);
        fileExporter = new FileExporter(queryService);
    }

    static void Main(string[] args)
    {
        InitializeData();

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
                case "1": 
                    displayService.ShowExcellentStudentsOnFirstSecondCourses();
                    displayService.ShowGroupAveragesForFirstSecondCourses();
                    break;
                case "2": 
                    displayService.ShowAllStudents(); 
                    break;
                case "3": 
                    fileExporter.SaveToFile();
                    Console.WriteLine($"Результаты сохранены в файл: result.txt");
                    break;
                case "4": 
                    return;
            }

            Console.WriteLine("\nНажмите Enter...");
            Console.ReadLine();
        }
    }
}
