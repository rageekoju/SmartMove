using SmartMove.Model;

namespace SmartMove.Components.Pages
{
    // The Transactions component handles all user interactions related to transaction management.
    // It allows users to view, create, update, delete, and search for transactions. 
    // The component interacts with backend services to fetch, filter, and modify transaction data.
    public partial class Transactions
    {
        // List of transactions currently displayed in the UI.
        private List<Transaction> DisplayedTransactions = new();

        // List of all unique tags extracted from transactions, used for filtering and categorization.
        private List<string> AllTags = new();

        // Represents the current transaction being worked on (either new or existing).
        private Transaction Transaction = new();

        // Holds the input for tags as a comma-separated string, to be processed into a list.
        private string TagsInput = "";

        // A flag to determine whether the component is in 'Edit' mode (for updating an existing transaction).
        private bool EditMode = false;

        // Default list of transaction categories/tags for quick selection.
        private List<string> DefaultTags = new List<string> { "Rent", "Fuel", "Food", "Groceries", "Monthly", "Yearly", "Clothes", "EMI", "Miscellaneous" };

        // Represents the selected default tag for filtering transactions.
        private string SelectedDefaultTag = "All";

        // Input field for a custom tag to be added alongside default tags.
        private string CustomTagInput = "";

        // Search term for filtering transactions based on title or description.
        private string SearchTerm = "";

        // Represents the selected transaction type (e.g., "Expense" or "Income") for filtering.
        private string SelectedType = "All";

        // Represents the selected tag for filtering transactions based on user-selected tags.
        private string SelectedTag = "All";

        // Start date for filtering transactions by date range.
        private DateTime? StartDate;
        private DateTime? EndDate;

        // The component’s initialization lifecycle method.
        // It fetches and loads all transactions into the component when it is first loaded.
        protected override async Task OnInitializedAsync()
        {
            await LoadTransactionsAsync();
        }

        // Loads all transactions from the backend and updates the list of displayed transactions.
        // Also extracts all unique tags from the transactions to populate the tag filter options.
        private async Task LoadTransactionsAsync()
        {
            // Fetch all transactions from the service
            DisplayedTransactions = await TransactionService.GetAllTransactionsAsync();

            // Extract unique tags from transactions, handling null tags gracefully.
            AllTags = DisplayedTransactions
                .SelectMany(t => t.Tags ?? new List<string>()) // Avoid null reference errors by using an empty list
                .Distinct() // Ensure there are no duplicate tags
                .ToList();
        }

        // Handles form submission for either creating a new transaction or updating an existing one.
        // Based on the 'EditMode' flag, it either creates or updates the transaction via the service.
        private async Task HandleSubmit()
        {
            // Convert the comma-separated tag input into a list of tags.
            Transaction.Tags = TagsInput
                .Split(',', StringSplitOptions.RemoveEmptyEntries) // Split by commas, ignoring empty entries
                .Select(tag => tag.Trim()) // Trim leading/trailing whitespace from tags
                .ToList();

            // Perform either create or update operation based on the mode.
            if (EditMode)
            {
                // Update the existing transaction
                await TransactionService.UpdateTransactionAsync(Transaction);
            }
            else
            {
                // Create a new transaction
                await TransactionService.CreateTransactionAsync(Transaction);
            }

            // Reload transactions after submission to reflect the changes
            await LoadTransactionsAsync();

            // Reset the form to prepare for the next transaction entry
            ResetForm();
        }

        // Prepares the form for editing an existing transaction by populating the fields.
        // Sets the 'EditMode' flag to true to allow modifications.
        private void UpdateTransaction(Transaction transaction)
        {
            // Populate the form with the transaction's data for editing
            Transaction = new Transaction
            {
                Id = transaction.Id,
                Title = transaction.Title,
                Amount = transaction.Amount,
                Type = transaction.Type,
                Date = transaction.Date,
                Notes = transaction.Notes,
                Tags = new List<string>(transaction.Tags ?? new List<string>()) // Safely handle null tags
            };

            // Populate the TagsInput field with a comma-separated list of the tags
            TagsInput = string.Join(", ", transaction.Tags ?? new List<string>());

            // Set the component in 'EditMode' for updating the transaction
            EditMode = true;
        }

        // Deletes a transaction from the database and refreshes the list of displayed transactions.
        private async Task DeleteTransaction(int id)
        {
            // Call the service to delete the transaction by ID
            await TransactionService.DeleteTransactionAsync(id);

            // Reload the transactions after deletion to reflect the changes
            await LoadTransactionsAsync();
        }

        // Resets the form to its initial state, clearing the input fields and resetting the transaction object.
        private void ResetForm()
        {
            Transaction = new Transaction(); // Clear the transaction data
            TagsInput = ""; // Clear the tags input field
            EditMode = false; // Reset edit mode to false for creating new transactions
        }

        // Searches for transactions based on selected filters, including tags, type, and date range.
        // It updates the displayed transactions based on the search criteria.
        private async Task SearchTransactions()
        {
            // Fetch all transactions first
            var allTransactions = await TransactionService.GetAllTransactionsAsync();

            // Filter by date range if both dates are provided
            if (StartDate.HasValue && EndDate.HasValue)
            {
                allTransactions = allTransactions
                    .Where(t => t.Date >= StartDate.Value && t.Date <= EndDate.Value)
                    .ToList();
            }

            // Additional filtering logic for tags, type, and search term
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                allTransactions = allTransactions
                    .Where(t => t.Title.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            if (SelectedType != "All")
            {
                allTransactions = allTransactions
                    .Where(t => t.Type == SelectedType)
                    .ToList();
            }
            if (SelectedDefaultTag != "All")
            {
                allTransactions = allTransactions
                    .Where(t => t.Tags != null && t.Tags.Contains(SelectedDefaultTag))
                    .ToList();
            }
            if (!string.IsNullOrWhiteSpace(CustomTagInput))
            {
                allTransactions = allTransactions
                    .Where(t => t.Tags != null && t.Tags.Contains(CustomTagInput))
                    .ToList();
            }

            // Update displayed transactions
            DisplayedTransactions = allTransactions;

            // Refresh the UI
            StateHasChanged();
        }

    }
}
