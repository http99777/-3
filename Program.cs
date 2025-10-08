
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileApp
{
    public class Student
    {
        public string LastName;
        public List<int> Grades = new List<int>();

        public Student(string lastName, List<int> grades)
        {
            LastName = lastName;
            Grades = grades;
        }

        public double AverageGrade => Grades.Count > 0 ? Grades.Average() : 0;
        public bool IsExcellent => Grades.Count > 0 && Grades.All(grade => grade == 5);
    }

    public class Group
    {
        public string Name;
        public List<Student> Students = new List<Student>();

        public Group(string name, List<Student> students)
        {
            Name = name;
            Students = students;
        }

        public double AverageGrade => Students.Count > 0 ? Students.Average(s => s.AverageGrade) : 0;
    }

    public class Course
    {
        public int Number;
        public List<Group> Groups = new List<Group>();

        public Course(int number, List<Group> groups)
        {
            Number = number;
            Groups = groups;
        }
    }

    public class Institute
    {
        public string Name;
        public List<Course> Courses = new List<Course>();

        public Institute(string name, List<Course> courses)
        {
            Name = name;
            Courses = courses;
        }
    }

    class Program
    {
        static List<Institute> institutes = new List<Institute>();

        static void Main()
        {
            InitializeData();

            while (true)
            {
                Console.WriteLine("1. Показать все данные");
                Console.WriteLine("2. Найти институт и курс с средним баллом >3.5");
                Console.WriteLine("3. Отличники 1-2 курсов с сортировкой групп по среднему баллу");
                Console.WriteLine("4. Сохранить все данные в файл");
                Console.WriteLine("5. Выход");
                Console.Write("Выберите пункт: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": ShowAllData(); break;
                    case "2": Two(); break;
                    case "3": Task15(); break;
                    case "4": SaveAllDataToFile(); break;
                    case "5": return;
                    default: Console.WriteLine("Нет такого пункта!"); break;
                }
            }
        }

        static void InitializeData()
        {
            var studentsIT1 = new List<Student>
            {
                new Student("Иванов", new List<int> {5, 5, 5, 5}),
                new Student("Петров", new List<int> {4, 5, 4, 4}),
                new Student("Сидоров", new List<int> {3, 4, 3, 4})
            };

            var studentsFIZ1 = new List<Student>
            {
                new Student("Николаев", new List<int> {5, 5, 5, 5}),
                new Student("Орлов", new List<int> {4, 4, 4, 4}),
                new Student("Андреев", new List<int> {2, 3, 3, 2})
            };

            var studentsIT2 = new List<Student>
            {
                new Student("Федоров", new List<int> {5, 5, 4, 5}),
                new Student("Жуков", new List<int> {5, 5, 5, 5}),
                new Student("Виноградов", new List<int> {3, 3, 4, 4})
            };

            var groupsCourse1 = new List<Group>
            {
                new Group("ИТ-101", studentsIT1),
                new Group("ФИЗ-101", studentsFIZ1)
            };

            var groupsCourse2 = new List<Group>
            {
                new Group("ИТ-201", studentsIT2)
            };

            var coursesIT = new List<Course>
            {
                new Course(1, new List<Group> { groupsCourse1[0] }),
                new Course(2, new List<Group> { groupsCourse2[0] })
            };

            var coursesPhysics = new List<Course>
            {
                new Course(1, new List<Group> { groupsCourse1[1] })
            };

            var itInstitute = new Institute("Институт информационных технологий", coursesIT);
            var physicsInstitute = new Institute("Физический институт", coursesPhysics);

            institutes.Add(itInstitute);
            institutes.Add(physicsInstitute);

            Console.WriteLine("Данные успешно инициализированы!");
            Console.WriteLine($"Загружено: {institutes.Count} института, " +
                            $"{institutes.Sum(i => i.Courses.Count)} курсов, " +
                            $"{institutes.Sum(i => i.Courses.Sum(c => c.Groups.Count))} групп, " +
                            $"{institutes.Sum(i => i.Courses.Sum(c => c.Groups.Sum(g => g.Students.Count)))} студентов");
            Console.WriteLine();
        }

        static void ShowAllData()
        {
            if (institutes.Count == 0)
            {
                Console.WriteLine("Данных нет!");
                return;
            }

            foreach (var institute in institutes)
            {
                Console.WriteLine($"Институт: {institute.Name}");
                foreach (var course in institute.Courses)
                {
                    Console.WriteLine($" Курс {course.Number}:");
                    foreach (var group in course.Groups)
                    {
                        Console.WriteLine($" Группа {group.Name} (средний балл: {group.AverageGrade:F2}):");
                        foreach (var student in group.Students)
                        {
                            Console.Write($" {student.LastName} - оценки: ");
                            foreach (var grade in student.Grades)
                            {
                                Console.Write($"{grade} ");
                            }
                            Console.WriteLine($"(средний: {student.AverageGrade:F2})");
                        }
                    }
                }
                Console.WriteLine();
            }
        }

        static void Two()
        {
            List<string> result = new List<string>();
            foreach (var institute in institutes)
            {
                foreach (var course in institute.Courses)
                {
                    int totalGrades = 0;
                    int gradesCount = 0;
                    foreach (var group in course.Groups)
                    {
                        foreach (var student in group.Students)
                        {
                            foreach (var grade in student.Grades)
                            {
                                totalGrades += grade;
                                gradesCount++;
                            }
                        }
                    }
                    if (gradesCount > 0)
                    {
                        double averageGrade = (double)totalGrades / gradesCount;
                        if (averageGrade >= 3.5)
                        {
                            result.Add($"{institute.Name}, курс {course.Number}, средний балл: {averageGrade:F2}");
                        }
                    }
                }
            }

            Console.WriteLine("Институты и курсы со средним баллом не менее 3.5:");
            Console.WriteLine("=================================================");

            if (result.Count == 0)
            {
                Console.WriteLine("Таких институтов и курсов нет");
            }
            else
            {
                foreach (string item in result)
                {
                    Console.WriteLine(item);
                }
            }
            Console.WriteLine();
        }

        static void Task15()
        {
            // Список для хранения информации о группах с отличниками 1-2 курсов
            var groupsWithExcellentStudents = new List<(Institute Institute, Course Course, Group Group, double AverageGrade, List<Student> ExcellentStudents)>();

            foreach (var institute in institutes)
            {
                foreach (var course in institute.Courses.Where(c => c.Number == 1 || c.Number == 2))
                {
                    foreach (var group in course.Groups)
                    {
                        var excellentStudents = group.Students.Where(s => s.IsExcellent).ToList();

                        if (excellentStudents.Any())
                        {
                            groupsWithExcellentStudents.Add((institute, course, group, group.AverageGrade, excellentStudents));
                        }
                    }
                }
            }

            var sortedGroups = groupsWithExcellentStudents.OrderByDescending(g => g.AverageGrade).ToList();

        
            Console.WriteLine("Отличники 1-2 курсов (группы отсортированы по среднему баллу):");
            Console.WriteLine("=================================================================");

            if (!sortedGroups.Any())
            {
                Console.WriteLine("Отличников на 1-2 курсах не найдено.");
                return;
            }

            foreach (var groupInfo in sortedGroups)
            {
                Console.WriteLine($"Институт: {groupInfo.Institute.Name}");
                Console.WriteLine($"Курс: {groupInfo.Course.Number}");
                Console.WriteLine($"Группа: {groupInfo.Group.Name}");
                Console.WriteLine($"Средний балл группы: {groupInfo.AverageGrade:F2}");
                Console.WriteLine("Отличники:");

                foreach (var student in groupInfo.ExcellentStudents)
                {
                    Console.WriteLine($"  - {student.LastName} (оценки: {string.Join(", ", student.Grades)})");
                }
                Console.WriteLine("-----------------------------------------------------------------");
            }

          
            try
            {
                using (StreamWriter writer = new StreamWriter("task15_results.txt", false, System.Text.Encoding.Default))
                {
                    writer.WriteLine("Отличники 1-2 курсов (группы отсортированы по среднему баллу):");
                    writer.WriteLine("=================================================================");

                    foreach (var groupInfo in sortedGroups)
                    {
                        writer.WriteLine($"Институт: {groupInfo.Institute.Name}");
                        writer.WriteLine($"Курс: {groupInfo.Course.Number}");
                        writer.WriteLine($"Группа: {groupInfo.Group.Name}");
                        writer.WriteLine($"Средний балл группы: {groupInfo.AverageGrade:F2}");
                        writer.WriteLine("Отличники:");

                        foreach (var student in groupInfo.ExcellentStudents)
                        {
                            writer.WriteLine($"  - {student.LastName} (оценки: {string.Join(", ", student.Grades)})");
                        }
                        writer.WriteLine("-----------------------------------------------------------------");
                    }
                }
                Console.WriteLine("Результаты сохранены в файл 'task15_results.txt'");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении в файл: {ex.Message}");
            }

            Console.WriteLine();
        }

        static void SaveAllDataToFile()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter("alldata.txt", false, System.Text.Encoding.Default))
                {
                    foreach (var institute in institutes)
                    {
                        writer.WriteLine($"Институт: {institute.Name}");
                        foreach (var course in institute.Courses)
                        {
                            writer.WriteLine($" Курс: {course.Number}");
                            foreach (var group in course.Groups)
                            {
                                writer.WriteLine($" Группа: {group.Name} (средний балл: {group.AverageGrade:F2})");
                                foreach (var student in group.Students)
                                {
                                    writer.Write($" Студент: {student.LastName} | Оценки: ");
                                    foreach (var grade in student.Grades)
                                    {
                                        writer.Write($"{grade} ");
                                    }
                                    writer.WriteLine($"(средний: {student.AverageGrade:F2})");
                                }
                            }
                        }
                        writer.WriteLine();
                    }
                }
                Console.WriteLine("Все данные сохранены в файл alldata.txt");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении: {ex.Message}");
            }

            Console.WriteLine();
        }
    }
}