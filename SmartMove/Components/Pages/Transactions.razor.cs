using SmartMove.Abstraction;
using SmartMove.Model;
using SmartMove.Service;

namespace SmartMove.Components.Pages
{
    public partial class Transactions
    {
        private List<Transaction> transactions = new();
        private Transaction Transaction = new();
        private bool EditMode = false;
        private string TagsInput = "";

        protected override async Task OnInitializedAsync()
        {
            await LoadTransactions();
        }

        private async Task LoadTransactions()
        {
            transactions = TransactionService.GetAllTransactions();
        }

        private async Task HandleSubmit()
        {
            // Parse tags input
            Transaction.Tags = TagsInput.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                        .Select(tag => tag.Trim())
                                        .ToList();

            if (EditMode)
            {
                await TransactionService.UpdateTransaction(Transaction);
            }
            else
            {
                await TransactionService.CreateTransaction(Transaction);
            }

            // Refresh the transaction list and reset form
            await LoadTransactions();
            ResetForm();
        }

        private void EditTransaction(Transaction transaction)
        {
            Transaction = new Transaction
            {
                Id = transaction.Id,
                Title = transaction.Title,
                Amount = transaction.Amount,
                Type = transaction.Type,
                Date = transaction.Date,
                Tags = new List<string>(transaction.Tags)
            };

            TagsInput = string.Join(", ", transaction.Tags);
            EditMode = true;
        }

        private async Task DeleteTransaction(int id)
        {
            await TransactionService.DeleteTransaction(id);
            await LoadTransactions();
        }

        private void ResetForm()
        {
            Transaction = new Transaction();
            TagsInput = "";
            EditMode = false;
        }
    }
}