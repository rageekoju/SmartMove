public class Transaction
{
    public int Id { get; set; }
    public string Title { get; set; }
    public decimal Amount { get; set; }  // Using decimal for financial calculations
    public string Type { get; set; }
    public DateTime Date { get; set; }
    public List<string> Tags { get; set; } = new List<string>();  // Ensure it's initialized
    public string Notes { get; set; }
    public bool IsCleared { get; set; }
}
