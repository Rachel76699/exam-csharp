using System;
using System.Text;
using App.Models;

namespace App.Print
{
    internal static class PrintResults
    {
        public static string Format(ReportResult result, bool includeExtendedSections = true)
        {
            StringBuilder output = new StringBuilder();

            output.AppendLine("FINES");
            output.AppendLine("reader_id,name,fine");
            foreach (ReaderFine item in result.Fines)
                output.AppendLine(item.Reader.Id + "," + RightToLeft(item.Reader.Name) + "," + item.Fine);

            output.AppendLine("FLAGGED");
            output.AppendLine("reader_id,name");
            foreach (Reader reader in result.Flagged)
                output.AppendLine(reader.Id + "," + RightToLeft(reader.Name));

            output.AppendLine("ERRORS");
            output.AppendLine("reader_id,name,error");
            foreach (ErrorRow error in result.Errors)
                output.AppendLine(error.ReaderId + "," + RightToLeft(error.Name) + "," + error.Message);

            output.AppendLine("TOTAL," + result.TotalFine);

            if (includeExtendedSections)
            {
                output.AppendLine("TOP_BOOKS");
                output.AppendLine("book,count");
                foreach (Book book in result.TopBooks)
                    output.AppendLine(RightToLeft(book.Name) + "," + book.Count);

                output.AppendLine("AT_RISK");
                output.AppendLine("reader_id,name");
                foreach (Reader reader in result.AtRisk)
                    output.AppendLine(reader.Id + "," + RightToLeft(reader.Name));
            }

            return output.ToString().TrimEnd('\r', '\n');
        }

        public static void Print(ReportResult result, bool includeExtendedSections = true)
        {
            Console.WriteLine(Format(result, includeExtendedSections));
        }

        private static string RightToLeft(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            foreach (char character in value)
            {
                if (character >= '\u0590' && character <= '\u05FF')
                    return "\u202B" + value + "\u202C";
            }

            return value;
        }
    }
}
