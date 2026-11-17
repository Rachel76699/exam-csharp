namespace App.Models
{
    public class Loan
    {
        public Reader Reader { get; set; } = new Reader();
        public string Book { get; set; } = "";
        public int LoanDay { get; set; }
        public int? ReturnDay { get; set; }
        public int Fine { get; set; }

        public bool IsReturned => ReturnDay.HasValue;
    }
}
