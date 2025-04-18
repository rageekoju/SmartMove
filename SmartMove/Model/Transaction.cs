public class Transaction
{
    // Unique identifier for the transaction
    public int Id { get; set; }

    // Title of the transaction 
    public string Title { get; set; }

    // Amount of money involved in the transaction 
    public decimal Amount { get; set; }

    // Type of transaction 
    public string Type { get; set; }

    // Date when the transaction took place
    public DateTime Date { get; set; } = DateTime.Today;

    // List of tags to categorize or label the transaction 
    // Ensured to be initialized to avoid null reference exceptions
    public List<string> Tags { get; set; } = new List<string>();

    // Additional notes related to the transaction 
    public string Notes { get; set; }

    // Flag to indicate whether the transaction has been cleared 
    public bool IsCleared { get; set; }
}
