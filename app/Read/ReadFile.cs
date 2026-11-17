using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace App.Read
{
    internal class ReadFile
    {
        static public int reportDay {  get; set; }
        static public List<string[]> rows {  get; set; }
        public ReadFile()
        {
            Console.OutputEncoding = new UTF8Encoding(false);
            string path = FindSample();
            string[] lines = File.ReadAllLines(path, Encoding.UTF8);
            reportDay = ReadReportDay(lines);
            rows = ReadRows(lines);

        }

            static public int ReadReportDay(string[] lines)
        {
            return int.Parse(lines[0].Split(':')[1].Trim());
        }

        // The lines after the two first ones, each split into its fields
        static public List<string[]> ReadRows(string[] lines)
        {
            List<string[]> rows = new List<string[]>();
            for (int i = 2; i < lines.Length; i++)
            {
                if (lines[i].Trim() == "") continue;
                string[] fields = lines[i].Split(',');
                for (int j = 0; j < fields.Length; j++) fields[j] = fields[j].Trim();
                rows.Add(fields);
            }
            return rows;
        }

        // Without a path, look for data/sample.txt next to the project, also when started from Visual Studio
        static public string FindSample()
        {
            DirectoryInfo dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir != null)
            {
                string candidate = Path.Combine(dir.FullName, "data", "sample.txt");
                if (File.Exists(candidate)) return candidate;
                dir = dir.Parent;
            }
            return Path.Combine("data", "sample.txt");
        }
    }
}
