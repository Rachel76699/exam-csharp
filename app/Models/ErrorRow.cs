namespace App.Models
{
    public class ErrorRow
    {
        public int LineNumber { get; set; }
        public string ReaderId { get; set; } = "";
        public string Name { get; set; } = "";
        public string Message { get; set; } = "";
        public string RawRow { get; set; } = "";
    }
}
