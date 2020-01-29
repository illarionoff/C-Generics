using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Generics
{
    public static class TextFileProcessor
    {
        public static List<T> LoadFromTextFile<T>(string filePath) where T : class, new()
        {
            var lines = System.IO.File.ReadAllLines(filePath).ToList();
            List<T> output = new List<T>();
            T entry = new T();
            var cols = entry.GetType().GetProperties();

            if (lines.Count < 2)
            {
                throw new IndexOutOfRangeException("File empty or missing");
            }

            var headers = lines[0].Split(',');

            lines.RemoveAt(0);

            foreach (var row in lines)
            {
                entry = new T();
                var vals = row.Split(',');

                for (var i = 0; i < headers.Length; i++)
                {
                    foreach (var col in cols)
                    {
                        if (col.Name == headers[i])
                        {
                            col.SetValue(entry, Convert.ChangeType(vals[i], col.PropertyType));
                        }
                    }
                }
                output.Add(entry);
            }

            return output;

        }

        public static void SaveTotextFile<T>(List<T> data, string filePath) where T : class
        {
            List<string> lines = new List<string>();
            StringBuilder line = new StringBuilder();

            if (data == null || data.Count == 0)
            {
                throw new ArgumentNullException("data", "At least one operator");
            }

            var cols = data[0].GetType().GetProperties();
            foreach (var col in cols)
            {
                line.Append(col.Name);
                line.Append(',');
            }
            lines.Add(line.ToString().Substring(0, line.Length -1));

            foreach (var row in data)
            {
                line = new StringBuilder();

                foreach (var col in cols)
                {
                    line.Append(col.GetValue(row));
                    line.Append(',');
                }

                lines.Add(line.ToString().Substring(0, line.Length - 1));
            }
            System.IO.File.WriteAllLines(filePath, lines);
        }
    }
    internal class Program
    {
        private static void Main(string[] args)
        {
            Demonstrate();
        }

        private static void Demonstrate()
        {
            var people = new List<Person>();
            var logs = new List<Log>();

            var peopleFile = @"C:\Temp\people.csv";
            var logsFile = @"C:\Temp\logs.csv";

            PopulateLists(people, logs);

            TextFileProcessor.SaveTotextFile(people, peopleFile);
            TextFileProcessor.SaveTotextFile(logs, logsFile);

            var newPeople = TextFileProcessor.LoadFromTextFile<Person>(peopleFile);
            var newLogs = TextFileProcessor.LoadFromTextFile<Log>(logsFile);

            foreach (var person in newPeople) Console.WriteLine(person.FirstName);

            foreach (var log in newLogs) Console.WriteLine(log.Title);
        }

        private static void PopulateLists(List<Person> people, List<Log> logs)
        {
            people.Add(new Person {FirstName = "John", Age = 32});
            people.Add(new Person {FirstName = "Mary", Age = 25});
            people.Add(new Person {FirstName = "Tammy", Age = 50});

            logs.Add(new Log {Title = "Big error"});
            logs.Add(new Log {Title = "Small error"});
            logs.Add(new Log {Title = "Fatal error"});
        }
    }
}