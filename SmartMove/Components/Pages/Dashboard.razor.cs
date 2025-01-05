namespace SmartMove.Components.Pages
{
    public partial class Dashboard
    {
        //This method is called when the dashboard page is opened
        protected override void OnInitialized()
        {
            CalculateTotals();
        }

        //Creating a transaction class with some properties
        public class Transaction
        {
            public string Description { get; set; }
            public DateTime Date { get; set; }
            public decimal Amount { get; set; }
            public string Type { get; set; } // Income, Expense, Debt
        }

        //Declaring some fields
        private Transaction newTransaction = new Transaction(); //Transaction class object
        private bool showModal = false; //a boolean value that determines wheather to show add transaction modal form or not

        //These fields are meant to hold the total values after CalculateTotals() method is called.
        private decimal totalIncome;
        private decimal totalExpense;
        private decimal totalDebt;

        /* Declaring and assigning value to a LIST of type Transaction. These values are set by default.
        The new values from modal form will get appended to last index of this list */
        private List<Transaction> transactions = new List<Transaction>()
    {
        new Transaction { Description = "Grocery Shopping", Date = DateTime.Parse("2024-12-25"), Amount = 150, Type = "Expense" },
        new Transaction { Description = "Freelance Payment", Date = DateTime.Parse("2024-12-24"), Amount = 500, Type = "Income" },
        new Transaction { Description = "Borrowed from Someone", Date = DateTime.Parse("2024-12-22"), Amount = 300, Type = "Debt" }
    };

        //this nethod is called when Add Transaction button from the dashboard is clicked
        private void OpenModal()
        {
            newTransaction = new Transaction();
            newTransaction.Date = DateTime.Now;
            showModal = true;
        }

        //This method can be called anytime we need to get total amt of each transaction type.
        private void CalculateTotals()
        {
            //LINQ method used with lamda expressions and extension methods
            totalIncome = transactions
                .Where(t => t.Type == "Income")
                .Sum(t => t.Amount);

            //TASK NO 1:*** use these totalExpense and totalDebt values instead of static HTML values
            totalExpense = transactions
                .Where(t => t.Type == "Expense")
                .Sum(t => t.Amount);

            totalDebt = transactions
                .Where(t => t.Type == "Debt")
                .Sum(t => t.Amount);
        }


        //this method is called to close the form modal
        private void CloseModal()
        {
            showModal = false;
        }

        //This method gets called from the modal button "Add"
        private void AddTransaction()
        {
            //TASK NO 2:*** Use try catch block for exception haldling***
            if (newTransaction != null && !string.IsNullOrWhiteSpace(newTransaction.Description))
            {
                transactions.Add(new Transaction
                {
                    Description = newTransaction.Description,
                    Date = newTransaction.Date == default ? DateTime.Now : newTransaction.Date,
                    Amount = newTransaction.Amount,
                    Type = newTransaction.Type
                });
                CalculateTotals(); // Update totals everytime after adding a new transaction.
            }
            CloseModal();
        }
    }
}