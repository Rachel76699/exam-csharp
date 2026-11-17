using System;
using System.IO;
using System.Text;
using App.Logic;
using App.Print;
using App.Read;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = new UTF8Encoding(false);
        string path = args.Length > 0 ? args[0] : ReadFile.FindSample();

        try
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("Input file was not found", path);

            PrintResults.Print(LogicFine.BuildReport(ReadFile.ReadReport(path)));
        }
        catch (Exception exception) when (exception is IOException || exception is FormatException)
        {
            Console.Error.WriteLine(exception.Message);
            Environment.ExitCode = 1;
        }
    }
}