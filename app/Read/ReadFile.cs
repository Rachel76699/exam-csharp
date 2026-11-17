using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using App.Models;

namespace App.Read
{
    internal class ReadFile
    {
        public static int reportDay { get; set; }
        public static List<string[]> rows { get; set; } = new List<string[]>();

        public ReadFile() : this(FindSample())
        {
        }

        public ReadFile(string path)
        {
            Console.OutputEncoding = new UTF8Encoding(false);
            string[] lines = File.ReadAllLines(path, Encoding.UTF8);
            reportDay = ReadReportDay(lines);
            rows = ReadRows(lines);
        }

        public static int ReadReportDay(string[] lines)
        {
            if (lines == null || lines.Length == 0)
                throw new FormatException("Missing report day");

            string[] parts = lines[0].Split(':', 2);
            if (parts.Length != 2 || !int.TryParse(parts[1].Trim(), out int day))
                throw new FormatException("Invalid report day");

            return day;
        }

        public static List<string[]> ReadRows(string[] lines)
        {
            List<string[]> result = new List<string[]>();
            if (lines == null)
                return result;

            for (int i = 2; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                    continue;

                string[] fields = lines[i].Split(',');
                for (int j = 0; j < fields.Length; j++)
                    fields[j] = fields[j].Trim();
                result.Add(fields);
            }

            return result;
        }

        public static ReportData ReadReport(string path)
        {
            string[] lines = File.ReadAllLines(path, Encoding.UTF8);
            ReportData report = new ReportData { ReportDay = ReadReportDay(lines) };

            for (int i = 2; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                    continue;

                string raw = lines[i];
                string[] fields = raw.Split(',');
                for (int j = 0; j < fields.Length; j++)
                    fields[j] = fields[j].Trim();

                if (TryParseLoan(fields, report.ReportDay, out Loan? loan, out string message))
                {
                    report.Loans.Add(loan!);
                }
                else
                {
                    report.Errors.Add(new ErrorRow
                    {
                        LineNumber = i + 1,
                        ReaderId = fields.Length > 0 ? fields[0] : "",
                        Name = fields.Length > 1 ? fields[1] : "",
                        Message = message,
                        RawRow = raw
                    });
                }
            }

            return report;
        }

        private static bool TryParseLoan(string[] fields, int reportDay, out Loan? loan, out string message)
        {
            loan = null;
            message = "";

            if (fields.Length != 6)
            {
                message = "wrong number of fields";
                return false;
            }

            if (!int.TryParse(fields[0], out int readerId))
            {
                message = "invalid reader_id";
                return false;
            }

            if (string.IsNullOrWhiteSpace(fields[1]) || string.IsNullOrWhiteSpace(fields[3]))
            {
                message = "missing reader name or book";
                return false;
            }

            string type = fields[2].ToLowerInvariant();
            if (type != "student" && type != "teacher" && type != "guest" && type != "alumna")
            {
                message = "invalid reader type";
                return false;
            }

            if (!int.TryParse(fields[4], out int loanDay))
            {
                message = "invalid loan_day";
                return false;
            }

            int? returnDay = null;
            if (!string.IsNullOrWhiteSpace(fields[5]))
            {
                if (!int.TryParse(fields[5], out int parsedReturnDay))
                {
                    message = "invalid return_day";
                    return false;
                }

                if (parsedReturnDay < loanDay)
                {
                    message = "return_day before loan_day";
                    return false;
                }

                returnDay = parsedReturnDay;
            }

            if (loanDay > reportDay)
            {
                message = "loan_day after report_day";
                return false;
            }

            loan = new Loan
            {
                Reader = new Reader { Id = readerId, Name = fields[1], Type = type },
                Book = fields[3],
                LoanDay = loanDay,
                ReturnDay = returnDay
            };
            return true;
        }

        public static string FindSample()
        {
            DirectoryInfo? dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir != null)
            {
                string candidate = Path.Combine(dir.FullName, "data", "sample.txt");
                if (File.Exists(candidate))
                    return candidate;
                dir = dir.Parent;
            }

            return Path.Combine("data", "sample.txt");
        }
    }
}