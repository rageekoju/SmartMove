using System.Text.Json;
using SmartMove.Model;

namespace SmartMove.Abstraction;

public class TransactionService
{
    private static readonly string FilePath = Path.Combine(FileSystem.AppDataDirectory, "transactions.json");


    public TransactionService()
    {
        // Initialize the file if it doesn't exist
        if (!File.Exists(FilePath))
        {
            File.WriteAllText(FilePath, JsonSerializer.Serialize(new List<Transaction>()));
        }
    }

    // Retrieve all transactions
    public List<Transaction> GetAllTransactions()
    {
        var json = File.ReadAllText(FilePath);
        return JsonSerializer.Deserialize<List<Transaction>>(json) ?? new List<Transaction>();
    }

    // Add a new transaction
    public async Task CreateTransaction(Transaction transaction)
    {
        var transactions = GetAllTransactions();
        transaction.Id = transactions.Any() ? transactions.Max(t => t.Id) + 1 : 1; // Generate new ID
        transactions.Add(transaction);
        SaveTransactions(transactions);
    }

    // Update an existing transaction
    public async Task UpdateTransaction(Transaction updatedTransaction)
    {
        var transactions = GetAllTransactions();
        var existingTransaction = transactions.FirstOrDefault(t => t.Id == updatedTransaction.Id);

        if (existingTransaction != null)
        {
            existingTransaction.Title = updatedTransaction.Title;
            existingTransaction.Amount = updatedTransaction.Amount;
            existingTransaction.Type = updatedTransaction.Type;
            existingTransaction.Date = updatedTransaction.Date;
            existingTransaction.Tags = updatedTransaction.Tags;

            SaveTransactions(transactions);
            // Update successful
        }

         // Transaction not found
    }

    // Delete a transaction
    public async Task DeleteTransaction(int id)
    {
        var transactions = GetAllTransactions();
        var transactionToDelete = transactions.FirstOrDefault(t => t.Id == id);

        if (transactionToDelete != null)
        {
            transactions.Remove(transactionToDelete);
            SaveTransactions(transactions);
              // Deletion successful
        }

        // Transaction not found
    }

    // Save transactions to JSON file
    private void SaveTransactions(List<Transaction> transactions)
    {
        var json = JsonSerializer.Serialize(transactions, new JsonSerializerOptions
        {
            WriteIndented = true // Makes the JSON human-readable
        });
        File.WriteAllText(FilePath, json);
    }
}
