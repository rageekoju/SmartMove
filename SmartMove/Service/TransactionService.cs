using System.Text.Json;
using System.Linq;

public class TransactionService
{
    // File path for storing transaction data in JSON format
    private static readonly string FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "transactions.json");

    // Constructor ensures the file exists or creates it if missing
    public TransactionService()
    {
        EnsureFileExists();
    }

    // Ensures the transaction data file exists, creating necessary directories and initializing the file if required
    private void EnsureFileExists()
    {
        var directory = Path.GetDirectoryName(FilePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // If the file doesn't exist, initialize it with an empty list of transactions
        if (!File.Exists(FilePath))
        {
            File.WriteAllText(FilePath, JsonSerializer.Serialize(new List<Transaction>()));
        }
    }

    // Creates a new transaction after checking if there is sufficient balance
    public async Task CreateTransactionAsync(Transaction transaction)
    {
        var transactions = await GetAllTransactionsAsync();

        // Generate a unique ID for the new transaction
        transaction.Id = transactions.Any() ? transactions.Max(t => t.Id) + 1 : 1;

        // Ensure sufficient balance for Debit or Pending Debt transactions
        if ((transaction.Type == "Debit" || transaction.Type == "Pending Debt") && !await IsSufficientBalanceAsync(transaction.Amount))
        {
            throw new InvalidOperationException("Insufficient balance for this transaction.");
        }

        // Add the new transaction to the list
        transactions.Add(transaction);

        // Automatically assign tags based on the transaction title
        AddDefaultTags(transaction);

        // Save the updated transaction list back to the file
        await SaveTransactionsAsync(transactions);
    }

    // Checks if the balance is sufficient to cover the requested amount
    private async Task<bool> IsSufficientBalanceAsync(decimal amount)
    {
        var dashboardData = await GetDashboardDataAsync();
        decimal availableBalance = dashboardData["Total Inflows"] - dashboardData["Total Outflows"] - dashboardData["Cleared Debt"];
        return availableBalance >= amount;
    }

    // Adds default tags to a transaction based on its title (e.g., "Rent", "Fuel", etc.)
    private void AddDefaultTags(Transaction transaction)
    {
        var tagMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "rent", "Rent" },
            { "fuel", "Fuel" },
            { "food", "Food" },
            { "groceries", "Groceries" },
            { "monthly", "Monthly" },
            { "yearly", "Yearly" },
            { "clothes", "Clothes" },
            { "emi", "EMI" },
            { "miscellaneous", "Miscellaneous" }
        };

        // Add tags to the transaction if the title matches any predefined keywords
        foreach (var DT in tagMapping)
        {
            if (transaction.Title.Contains(DT.Key))
            {
                transaction.Tags.Add(DT.Value);
            }
        }
    }

    // Retrieves all transactions from the JSON file
    public async Task<List<Transaction>> GetAllTransactionsAsync()
    {
        try
        {
            // Read the JSON data from the file
            var json = await File.ReadAllTextAsync(FilePath);

            // Deserialize the data into a list of transactions
            var transactions = JsonSerializer.Deserialize<List<Transaction>>(json) ?? new List<Transaction>();

            // Ensure each transaction has a valid Tags list (even if it was null in the data)
            foreach (var transaction in transactions)
            {
                transaction.Tags ??= new List<string>();
            }
            return transactions;
        }
        catch (Exception ex)
        {
            // Log any errors while reading the file
            Console.WriteLine($"Error reading transactions: {ex.Message}");
            return new List<Transaction>();
        }
    }

    // Retrieves transactions filtered by a specific date range
    public async Task<List<Transaction>> GetFilteredTransactionsAsync(DateTime startDate, DateTime endDate)
    {
        var transactions = await GetAllTransactionsAsync();
        return transactions.Where(t => t.Date >= startDate && t.Date <= endDate).ToList();
    }

    // Searches transactions by tags, type, and other criteria
    public async Task<List<Transaction>> SearchTransactionsByTagsAsync(string searchTerm, string type, List<string> tags)
    {
        var transactions = await GetAllTransactionsAsync();
        var filteredTransactions = transactions
            .Where(t =>
                (string.IsNullOrEmpty(searchTerm) || t.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) &&
                (type == "All" || t.Type.Equals(type, StringComparison.OrdinalIgnoreCase)) &&
                (tags.Count == 0 || t.Tags.Any(tag => tags.Contains(tag))) 
            )
            .ToList();

        return filteredTransactions;
    }

    // Updates an existing transaction's details
    public async Task UpdateTransactionAsync(Transaction updatedTransaction)
    {
        var transactions = await GetAllTransactionsAsync();
        var existingTransaction = transactions.FirstOrDefault(t => t.Id == updatedTransaction.Id);
        if (existingTransaction != null)
        {
            existingTransaction.Title = updatedTransaction.Title;
            existingTransaction.Amount = updatedTransaction.Amount;
            existingTransaction.Type = updatedTransaction.Type;
            existingTransaction.Date = updatedTransaction.Date;
            existingTransaction.Tags = updatedTransaction.Tags ?? new List<string>();
            existingTransaction.Notes = updatedTransaction.Notes;

            // Save the updated list back to the file
            await SaveTransactionsAsync(transactions);
        }
    }

    // Deletes a transaction based on its ID
    public async Task DeleteTransactionAsync(int id)
    {
        var transactions = await GetAllTransactionsAsync();
        var transaction = transactions.FirstOrDefault(t => t.Id == id);
        if (transaction != null)
        {
            // Remove the transaction and save the updated list
            transactions.Remove(transaction);
            await SaveTransactionsAsync(transactions);
        }
    }

    // Retrieves dashboard data (total inflows, outflows, debts)
    public async Task<Dictionary<string, decimal>> GetDashboardDataAsync()
    {
        var transactions = await GetAllTransactionsAsync();
        var totalInflows = transactions.Where(t => t.Type == "Credit").Sum(t => t.Amount);
        var totalOutflows = transactions.Where(t => t.Type == "Debit").Sum(t => t.Amount);
        var clearedDebt = transactions.Where(t => t.Type == "Cleared Debt").Sum(t => t.Amount);
        var pendingDebt = transactions.Where(t => t.Type == "Pending Debt").Sum(t => t.Amount);
        var totalDebt = clearedDebt + pendingDebt;

        // Return the dashboard data as a dictionary
        return new Dictionary<string, decimal>
        {
            { "Total Inflows", totalInflows },
            { "Total Outflows", totalOutflows },
            { "Cleared Debt", clearedDebt },
            { "Pending Debt", pendingDebt },
            { "Total Debt", totalDebt }
        };
    }

    // Retrieves the top 5 transactions, either highest or lowest
    public async Task<List<Transaction>> GetTopTransactionsAsync(bool highest = true)
    {
        var transactions = await GetAllTransactionsAsync();
        return highest
            ? transactions.OrderByDescending(t => t.Amount).Take(5).ToList()
            : transactions.OrderBy(t => t.Amount).Take(5).ToList();
    }

    // Saves the list of transactions back to the JSON file
    private async Task SaveTransactionsAsync(List<Transaction> transactions)
    {
        try
        {
            // Serialize the transaction list with indents for readability
            var json = JsonSerializer.Serialize(transactions, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(FilePath, json);
        }
        catch (Exception ex)
        {
            // Log any errors during the save process
            Console.WriteLine($"Error saving transactions: {ex.Message}");
        }
    }

    // Loads and displays dashboard data, including top transactions
    public async Task LoadDashboardData()
    {
        try
        {
            var dashboardData = await GetDashboardDataAsync();
            decimal totalInflows = dashboardData["Total Inflows"];
            decimal totalOutflows = dashboardData["Total Outflows"];
            decimal clearedDebt = dashboardData["Cleared Debt"];
            decimal pendingDebt = dashboardData["Pending Debt"];
            decimal totalDebt = dashboardData["Total Debt"];
            decimal availableBalance = totalInflows - totalOutflows - clearedDebt;

            var topTransactions = await GetTopTransactionsAsync(highest: true);
            Console.WriteLine($"Total Inflows: {totalInflows}, Total Outflows: {totalOutflows}, Cleared Debt: {clearedDebt}, Pending Debt: {pendingDebt}, Total Debt: {totalDebt}");
            Console.WriteLine($"Available Balance: {availableBalance}");

            // Display the top transactions
            foreach (var transaction in topTransactions)
            {
                Console.WriteLine($"{transaction.Title} - {transaction.Amount} ({transaction.Type})");
            }
        }
        catch (Exception ex)
        {
            // Log any errors during the dashboard data loading
            Console.WriteLine($"Error loading dashboard data: {ex.Message}");
        }
    }
}
