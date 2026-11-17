using System.Collections.Generic;

namespace App.Models
{
    public class ReportData
    {
        public int ReportDay { get; set; }
        public List<Loan> Loans { get; } = new List<Loan>();
        public List<ErrorRow> Errors { get; } = new List<ErrorRow>();
    }
}
