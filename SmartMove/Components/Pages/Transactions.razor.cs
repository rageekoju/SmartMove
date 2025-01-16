using SmartMove.Model;

namespace SmartMove.Components.Pages
{
    public partial class Transactions
    {
        private List<Transaction> DisplayedTransactions = new();
        private List<string> AllTags = new();
        private Transaction Transaction = new();
        private string TagsInput = "";
        private bool EditMode = false;

        private string SearchTerm = "";
        private string SelectedType = "All";
        private string SelectedTag = "All";
        private DateTime? StartDate = null;
        private DateTime? EndDate = null;

        protected override async Task OnInitializedAsync()
        {
            await LoadTransactionsAsync();
        }

        private async Task LoadTransactionsAsync()
        {
            DisplayedTransactions = await TransactionService.GetAllTransactionsAsync();
            AllTags = DisplayedTransactions
                .SelectMany(t => t.Tags ?? new List<string>())
                .Distinct()
                .ToList();
        }

        private async Task HandleSubmit()
        {
            Transaction.Tags = TagsInput
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(tag => tag.Trim())
                .ToList();

            if (EditMode)
            {
                await TransactionService.UpdateTransactionAsync(Transaction);
            }
            else
            {
                await TransactionService.CreateTransactionAsync(Transaction);
            }

            await LoadTransactionsAsync();
            ResetForm();
        }

        private void UpdateTransaction(Transaction transaction)
        {
            Transaction = new Transaction
            {
                Id = transaction.Id,
                Title = transaction.Title,
                Amount = transaction.Amount,
                Type = transaction.Type,
                Date = transaction.Date,
                Tags = new List<string>(transaction.Tags ?? new List<string>())
            };

            TagsInput = string.Join(", ", transaction.Tags ?? new List<string>());
            EditMode = true;
        }

        private async Task DeleteTransaction(int id)
        {
            await TransactionService.DeleteTransactionAsync(id);
            await LoadTransactionsAsync();
        }

        private void ResetForm()
        {
            Transaction = new Transaction();
            TagsInput = "";
            EditMode = false;
        }
        private void ExportTransactions()
        {
            // Add logic to export transactions
            Console.WriteLine("Exporting transactions...");
        }
        private async Task SearchTransactions()
        {
            DisplayedTransactions = await TransactionService.SearchTransactionsAsync(SearchTerm, SelectedType, SelectedTag, EndDate);
        }
    }
    }