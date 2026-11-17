using System;
using System.Collections.Generic;
using System.Linq;
using App.Models;

namespace App.Logic
{
    internal class LogicFine
    {
        public static ReportResult BuildReport(ReportData report)
        {
            ReportResult result = new ReportResult();
            if (report == null)
                return result;

            result.Errors.AddRange(report.Errors);

            HashSet<int> flaggedReaderIds = new HashSet<int>();
            Dictionary<int, ReaderFine> finesByReader = new Dictionary<int, ReaderFine>();

            foreach (Loan loan in report.Loans)
            {
                int fine = CalculateLoanFine(loan, report.ReportDay);
                if (finesByReader.TryGetValue(loan.Reader.Id, out ReaderFine? readerFine))
                {
                    readerFine.Fine += fine;
                }
                else
                {
                    finesByReader.Add(loan.Reader.Id, new ReaderFine
                    {
                        Reader = loan.Reader,
                        Fine = fine
                    });
                }

                result.TotalFine += fine;

                if (!loan.IsReturned && report.ReportDay - loan.LoanDay > 60 && flaggedReaderIds.Add(loan.Reader.Id))
                    result.Flagged.Add(loan.Reader);

                if (!loan.IsReturned && IsOverdue(loan, report.ReportDay) && !result.AtRisk.Any(reader => reader.Id == loan.Reader.Id))
                    result.AtRisk.Add(loan.Reader);
            }

            result.Fines.AddRange(finesByReader.Values
                .OrderByDescending(readerFine => readerFine.Fine)
                .ThenBy(readerFine => readerFine.Reader.Id));

            result.Flagged.Sort((left, right) => left.Id.CompareTo(right.Id));
            result.AtRisk.Sort((left, right) => left.Id.CompareTo(right.Id));

            foreach (IGrouping<string, Loan> bookLoans in report.Loans
                .GroupBy(loan => loan.Book)
                .OrderByDescending(group => group.Count())
                .ThenBy(group => group.Key, StringComparer.Ordinal)
                .Take(3))
                result.TopBooks.Add(new Book { Name = bookLoans.Key, Count = bookLoans.Count() });

            return result;
        }

        public static List<int> CalcFines(List<string[]> data, int reportDay)
        {
            List<int> fines = new List<int>();
            if (data == null)
                return fines;

            foreach (string[] row in data)
                fines.Add(CalcFine(row, reportDay));

            return fines;
        }

        public static int FineStudent(string[] row, int reportDay)
        {
            return CalculateFine(row, reportDay, 14, 1, 40);
        }

        public static int FineTeacher(string[] row, int reportDay)
        {
            return CalculateFine(row, reportDay, 60, 0, 0);
        }

        public static int FineGuest(string[] row, int reportDay)
        {
            if (row == null || row.Length < 5 || !int.TryParse(row[4], out int loanDay))
                return 0;

            int endDay = reportDay;
            if (row.Length >= 6 && !string.IsNullOrWhiteSpace(row[5]) && !int.TryParse(row[5], out endDay))
                return 0;

            return CalculateGuestFine(endDay - loanDay);
        }

        public static int FineAlumna(string[] row, int reportDay)
        {
            if (row == null || row.Length < 5 || !int.TryParse(row[4], out int loanDay))
                return 0;

            int endDay = reportDay;
            if (row.Length >= 6 && !string.IsNullOrWhiteSpace(row[5]) && !int.TryParse(row[5], out endDay))
                return 0;

            return CalculateAlumnaFine(endDay - loanDay);
        }

        private static int CalcFine(string[] row, int reportDay)
        {
            if (row == null || row.Length < 5 || string.IsNullOrWhiteSpace(row[2]))
                return 0;

            switch (row[2].Trim().ToLowerInvariant())
            {
                case "student":
                    return FineStudent(row, reportDay);
                case "teacher":
                    return FineTeacher(row, reportDay);
                case "guest":
                    return FineGuest(row, reportDay);
                case "alumna":
                    return FineAlumna(row, reportDay);
                default:
                    return 0;
            }
        }

        private static int CalculateLoanFine(Loan loan, int reportDay)
        {
            int allowedDays;
            int finePerDay;
            int maximumFine;

            switch (loan.Reader.Type)
            {
                case "student":
                    allowedDays = 14;
                    finePerDay = 1;
                    maximumFine = 40;
                    break;
                case "teacher":
                    allowedDays = 60;
                    finePerDay = 0;
                    maximumFine = 0;
                    break;
                case "guest":
                    int guestEndDay = loan.ReturnDay ?? reportDay;
                    return CalculateGuestFine(guestEndDay - loan.LoanDay);
                case "alumna":
                    int alumnaEndDay = loan.ReturnDay ?? reportDay;
                    return CalculateAlumnaFine(alumnaEndDay - loan.LoanDay);
                default:
                    return 0;
            }

            int endDay = loan.ReturnDay ?? reportDay;
            return CalculateFine(endDay - loan.LoanDay, allowedDays, finePerDay, maximumFine);
        }

        private static bool IsOverdue(Loan loan, int reportDay)
        {
            return CalculateLoanFine(loan, reportDay) > 0;
        }

        private static int CalculateFine(string[] row, int reportDay, int allowedDays, int finePerDay, int maximumFine)
        {
            if (row == null || row.Length < 5 || !int.TryParse(row[4], out int loanDay))
                return 0;

            int endDay = reportDay;
            if (row.Length >= 6 && !string.IsNullOrWhiteSpace(row[5]) && !int.TryParse(row[5], out endDay))
                return 0;

            return CalculateFine(endDay - loanDay, allowedDays, finePerDay, maximumFine);
        }

        private static int CalculateFine(int loanDays, int allowedDays, int finePerDay, int maximumFine)
        {
            if (finePerDay == 0 || loanDays <= allowedDays)
                return 0;

            int fine = (loanDays - allowedDays) * finePerDay;
            return maximumFine > 0 ? Math.Min(fine, maximumFine) : fine;
        }

        private static int CalculateGuestFine(int loanDays)
        {
            if (loanDays <= 7)
                return 0;

            int lateDays = loanDays - 7;
            return (lateDays * 5 + 1) / 2;
        }

        private static int CalculateAlumnaFine(int loanDays)
        {
            if (loanDays <= 10)
                return 0;

            int lateDays = loanDays - 10;
            return Math.Min(25, (lateDays * 3 + 1) / 2);
        }
    }
}