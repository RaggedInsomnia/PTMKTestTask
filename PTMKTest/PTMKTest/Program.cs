using AgeCalculator;
using System.Globalization;
using System.Text;
using System.Diagnostics;

namespace PTMKTest
{
    public class App
    {
        private static void Main(string[] args)
        {
            int var = -1;
            while (var != 0)
            {
                var = Menu();
                switch (var)
                {
                    case 1:
                        Console.Clear();
                        string name = string.Empty;
                        DateTime date;
                        char sex = ' ';
                        Console.WriteLine("Input FIO: ");  //ввод данных человека с проверкой ввода на корректность
                        name = Console.ReadLine();
                        while (name.Equals(string.Empty) || name.Any(x => char.IsDigit(x)))
                        {
                            Console.WriteLine("Invalid input! Input FIO again: ");
                            name = Console.ReadLine();
                        }
                        Console.WriteLine("Input Date of Birth: ");
                        while (!DateTime.TryParse(Console.ReadLine(), out date))
                        {
                            Console.WriteLine("Invalid input! Input DoB again: ");
                        }
                        Console.WriteLine("Input Sex: ");
                        sex = Console.ReadLine().FirstOrDefault();
                        while (char.IsDigit(sex) || sex.Equals(' '))
                        {
                            Console.WriteLine("Invalid input! Input Sex again: ");
                            sex = Console.ReadLine().FirstOrDefault();
                        }
                        Person person = new()
                        {
                            Name = name,
                            DateofBirth = date,
                            Sex = sex
                        };
                        using (ApplicationContext db = new())  //запись нового объекта в БД
                        {
                            db.Add(person);
                            db.SaveChanges();
                        }
                        Console.WriteLine("Person added\nPress any button to continue");
                        Console.ReadKey();
                        break;
                    case 2:
                        Console.Clear();
                        List<Person> PersonList;
                        using (ApplicationContext db = new())  //получение списка уникальных объектов из БД и их вывод с сортировкой по имени
                        {
                            PersonList = db.Persons.Distinct().OrderBy(x => x.Name).ToList();
                        }
                        foreach (var per in PersonList)
                        {
                            Console.WriteLine($"Name: {per.Name}\nAge: {new Age(per.DateofBirth, DateTime.Today).Years}\nSex: {per.Sex}\n");
                        }
                        Console.WriteLine("Press any button to continue");
                        Console.ReadKey();
                        break;
                    case 3:
                        using (ApplicationContext db = new())  //генерация объектов и запись их в БД
                        {
                            for (int i = 0; i < 1000 /*1000000*/; i++)
                            {
                                Person newperson = new()
                                {
                                    Name = GenerateFIO(),
                                    DateofBirth = DateTime.Today.AddDays(new Random().Next(-18250, 0)),
                                    Sex = new Random().Next(1, 3) == 1 ? 'M' : 'F'
                                };
                                db.Persons.Add(newperson);
                            }
                            db.SaveChanges();
                        }
                        break;
                    case 4:
                        using (ApplicationContext db = new())  //генерация 100 объектов, первая буква имени меняется на F
                        {
                            for (int i = 0; i < 100; i++)
                            {
                                Person newperson = new()
                                {
                                    Name = GenerateFIO(),
                                    DateofBirth = DateTime.Today.AddDays(new Random().Next(-18250, 0)),
                                    Sex = 'M'
                                };
                                newperson.Name = newperson.Name.Remove(0, 1);
                                newperson.Name = newperson.Name.Insert(0, 'F'.ToString());
                                db.Persons.Add(newperson);
                            }
                            db.SaveChanges();
                        }
                        break;
                    case 5:
                        Stopwatch stopwatch = new(); 
                        List<Person> MalesList;
                        stopwatch.Start();  //запуск таймера
                        using (ApplicationContext db = new())  //получение подходящих объектов
                        {
                            Console.Clear();
                            MalesList = (from per in db.Persons where per.Sex.Equals('M') && per.Name.FirstOrDefault().Equals('F') select per).ToList();//db.Persons.Where(x => x.Sex.Equals('M') && x.Name.First().Equals('F')).Distinct().OrderBy(x => x.Name).ToList();
                        }
                        foreach(var male in MalesList)  //вывод объектов
                        {
                            Console.WriteLine($"Name: {male.Name}\nAge: {new Age(male.DateofBirth, DateTime.Today).Years}\nSex: {male.Sex}\n");
                        }
                        stopwatch.Stop();  //остановка таймера и вывод результата
                        Console.WriteLine($"Time result: {stopwatch.Elapsed.TotalMilliseconds} ms");
                        Console.WriteLine("Press any button to continue");
                        Console.ReadKey();
                        break;
                    case 9:
                        using (ApplicationContext db = new())  //дополнительный пункт для очищения БД
                        {
                            db.Persons.RemoveRange(db.Persons);
                            db.SaveChanges();
                        }
                        break;
                    case 0:
                        break;
                    default:
                        Console.WriteLine("Wrong number, choose again");
                        break;

                }
            }

        }
        private static int Menu() //вывод меню для выбора действия
        {
            int var = -1;
            Console.Clear();
            Console.WriteLine("Choose option:\n1. Add Person\n2. Print unique objects\n3. Generate 1000000 records\n4. Generate 100 males\n5. Find males with 'F' starting name\n9. CLEAR\n0. Exit");
            while (!int.TryParse(Console.ReadLine(), out var))
                Console.WriteLine("Invalid format, try again: ");
            return var;
        }

        private static string GenerateFIO()  //генерация имени
        {
            Random rand = new();
            char[] letters = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
            StringBuilder res = new();
            for (int i = 0; i < 3; i++)
            {
                int wordlength = rand.Next(3, 10);  //генерация трех слов длиной от 3 до 10 символов
                for (int j = 0; j < wordlength; j++)
                {
                    res.Append(letters[rand.Next(letters.Length)]);
                }
                res.Append(' ');
            }
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            return textInfo.ToTitleCase(res.ToString());  //первая буква каждого слова становится заглавной
        }

    }

}