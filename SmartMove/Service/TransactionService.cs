using System.Text.Json;
using System.Linq;

public class TransactionService
{
    private static readonly string FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "transactions.json");

    public TransactionService()
    {
        EnsureFileExists();
    }

    private void EnsureFileExists()
    {
        var directory = Path.GetDirectoryName(FilePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        if (!File.Exists(FilePath))
        {
            File.WriteAllText(FilePath, JsonSerializer.Serialize(new List<Transaction>()));
        }
    }

    // Add a transaction with sufficient balance check
    public async Task CreateTransactionAsync(Transaction transaction)
    {
        var transactions = await GetAllTransactionsAsync();

        // Generate unique ID
        transaction.Id = transactions.Any() ? transactions.Max(t => t.Id) + 1 : 1;

        // Check for sufficient balance if the transaction is a Debit or Pending Debt
        if ((transaction.Type == "Debit" || transaction.Type == "Pending Debt") && !await IsSufficientBalanceAsync(transaction.Amount))
        {
            throw new InvalidOperationException("Insufficient balance for this transaction.");
        }

        // Add the transaction
        transactions.Add(transaction);
        AddDefaultTags(transaction);

        await SaveTransactionsAsync(transactions);
    }

    private async Task<bool> IsSufficientBalanceAsync(decimal amount)
    {
        var dashboardData = await GetDashboardDataAsync();
        decimal availableBalance = dashboardData["Total Inflows"] - dashboardData["Total Outflows"] - dashboardData["Cleared Debt"];
        return availableBalance >= amount;
    }

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

        foreach (var DT in tagMapping)
        {
            if (transaction.Title.Contains(DT.Key))
            {
                transaction.Tags.Add(DT.Value);
            }
        }
    }

    public async Task<List<Transaction>> GetAllTransactionsAsync()
    {
        try
        {
            var json = await File.ReadAllTextAsync(FilePath);
            var transactions = JsonSerializer.Deserialize<List<Transaction>>(json) ?? new List<Transaction>();
            foreach (var transaction in transactions)
            {
                transaction.Tags ??= new List<string>();
            }
            return transactions;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading transactions: {ex.Message}");
            return new List<Transaction>();
        }
    }

    public async Task<List<Transaction>> GetFilteredTransactionsAsync(DateTime startDate, DateTime endDate)
    {
        var transactions = await GetAllTransactionsAsync();
        return transactions.Where(t => t.Date >= startDate && t.Date <= endDate).ToList();
    }

    public async Task<List<Transaction>> SearchTransactionsAsync(string searchTerm, string type, string tag, DateTime? endDate)
    {
        var transactions = await GetAllTransactionsAsync();
        var filteredTransactions = transactions
            .Where(t =>
                (string.IsNullOrEmpty(searchTerm) || t.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) &&
                (type == "All" || t.Type.Equals(type, StringComparison.OrdinalIgnoreCase)) &&
                (tag == "All" || (t.Tags != null && t.Tags.Contains(tag))) &&
                (!endDate.HasValue || t.Date <= endDate)
            )
            .ToList();

        return filteredTransactions;
    }

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

            await SaveTransactionsAsync(transactions);
        }
    }

    public async Task DeleteTransactionAsync(int id)
    {
        var transactions = await GetAllTransactionsAsync();
        var transaction = transactions.FirstOrDefault(t => t.Id == id);
        if (transaction != null)
        {
            transactions.Remove(transaction);
            await SaveTransactionsAsync(transactions);
        }
    }

    public async Task<Dictionary<string, decimal>> GetDashboardDataAsync()
    {
        var transactions = await GetAllTransactionsAsync();
        var totalInflows = transactions.Where(t => t.Type == "Credit").Sum(t => t.Amount);
        var totalOutflows = transactions.Where(t => t.Type == "Debit").Sum(t => t.Amount);
        var clearedDebt = transactions.Where(t => t.Type == "Cleared Debt").Sum(t => t.Amount);
        var pendingDebt = transactions.Where(t => t.Type == "Pending Debt").Sum(t => t.Amount);
        var totalDebt = clearedDebt + pendingDebt;

        return new Dictionary<string, decimal>
        {
            { "Total Inflows", totalInflows },
            { "Total Outflows", totalOutflows },
            { "Cleared Debt", clearedDebt },
            { "Pending Debt", pendingDebt },
            { "Total Debt", totalDebt }
        };
    }

    public async Task<List<Transaction>> GetTopTransactionsAsync(bool highest = true)
    {
        var transactions = await GetAllTransactionsAsync();
        return highest
            ? transactions.OrderByDescending(t => t.Amount).Take(5).ToList()
            : transactions.OrderBy(t => t.Amount).Take(5).ToList();
    }

    private async Task SaveTransactionsAsync(List<Transaction> transactions)
    {
        try
        {
            var json = JsonSerializer.Serialize(transactions, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(FilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving transactions: {ex.Message}");
        }
    }

    public async Task ExportTransactionsToCsvAsync(string csvFilePath)
    {
        try
        {
            var transactions = await GetAllTransactionsAsync();
            var lines = new List<string> { "Title,Amount,Type,Date,Tags,Notes" };
            lines.AddRange(transactions.Select(t =>
                $"{t.Title},{t.Amount},{t.Type},{t.Date:yyyy-MM-dd},{string.Join(";", t.Tags)},{t.Notes}"));

            await File.WriteAllLinesAsync(csvFilePath, lines);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error exporting transactions to CSV: {ex.Message}");
        }
    }

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

            foreach (var transaction in topTransactions)
            {
                Console.WriteLine($"{transaction.Title} - {transaction.Amount} ({transaction.Type})");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading dashboard data: {ex.Message}");
        }
    }
}
