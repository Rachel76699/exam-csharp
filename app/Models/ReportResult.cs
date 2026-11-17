using System.Collections.Generic;

namespace App.Models
{
    public class ReportResult
    {
        public List<ReaderFine> Fines { get; } = new List<ReaderFine>();
        public List<Reader> Flagged { get; } = new List<Reader>();
        public List<ErrorRow> Errors { get; } = new List<ErrorRow>();
        public List<Book> TopBooks { get; } = new List<Book>();
        public List<Reader> AtRisk { get; } = new List<Reader>();
        public int TotalFine { get; set; }
    }
}
