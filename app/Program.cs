using App.Logic;
using App.Read;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


// The exam program. It gets the path of an input file, and prints the report.
// Run it from the exam folder:  python check/run_checks.py run
// Without a path, it reads data/sample.txt.
class Program
{
    static void Main(string[] args)
    {
        //// Hebrew is printed as UTF-8, also on Windows
        //Console.OutputEncoding = new UTF8Encoding(false);
        //string path = args.Length > 0 ? args[0] : ReadFile.FindSample();
        //string[] lines = File.ReadAllLines(path, Encoding.UTF8);
        //int reportDay = ReadFile.ReadReportDay(lines);
        //List<string[]> rows = ReadFile.ReadRows(lines);
        //Console.Error.WriteLine("skeleton ok: read " + Path.GetFileName(path) + " - report day " + reportDay + " - " + rows.Count + " rows");
        // Write your code here. 'reportDay' and 'rows' are ready.


        ReadFile r = new ReadFile();
        List<string[]> data = ReadFile.rows;
        int reportDay = ReadFile.reportDay;


        List<int> Fines = LogicFine.CalcFines(data, reportDay);

        foreach (var item in Fines)
        {
            Console.WriteLine(item);
        }



        Console.WriteLine();




    }

}
