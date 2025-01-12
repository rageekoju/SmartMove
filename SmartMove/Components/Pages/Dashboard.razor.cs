namespace SmartMove.Components.Pages
{
    public partial class Dashboard
    {
        private Transaction newTransaction = new Transaction();
        private decimal totalIncome = 0;
        private decimal totalExpense = 0;
        private decimal totalDebt = 0;
        private decimal clearedDebts = 0;
        private decimal pendingDebts = 0;

        private List<Transaction> transactions = new List<Transaction>
    {
        new Transaction { Description = "Salary", Date = DateTime.Now, Amount = 50000, Type = "Income" },
        new Transaction { Description = "Grocery", Date = DateTime.Now, Amount = 10000, Type = "Expense" },
        new Transaction { Description = "Loan", Date = DateTime.Now, Amount = 20000, Type = "Debt" },
        new Transaction { Description = "Debt Paid", Date = DateTime.Now, Amount = 10000, Type = "Cleared Debt" }
    };

        private double[] chartData = { 50, 30, 20 };
        private string[] chartLabels = { "Income", "Expense", "Debt" };

        protected override void OnInitialized()
        {
            CalculateTotals();
        }

        private void AddTransaction()
        {
            transactions.Add(new Transaction
            {
                Description = newTransaction.Description,
                Date = newTransaction.Date == default ? DateTime.Now : newTransaction.Date,
                Amount = newTransaction.Amount,
                Type = newTransaction.Type
            });

            CalculateTotals();
            UpdateChartData();
            newTransaction = new Transaction();
        }

        private void CalculateTotals()
        {
            totalIncome = transactions.Where(t => t.Type == "Income").Sum(t => t.Amount);
            totalExpense = transactions.Where(t => t.Type == "Expense").Sum(t => t.Amount);
            totalDebt = transactions.Where(t => t.Type == "Debt").Sum(t => t.Amount);
            clearedDebts = transactions.Where(t => t.Type == "Cleared Debt").Sum(t => t.Amount);
            pendingDebts = totalDebt - clearedDebts;
        }

        private void UpdateChartData()
        {
            chartData = new double[]
            {
            (double)totalIncome,
            (double)totalExpense,
            (double)totalDebt
            };
        }

        public class Transaction
        {
            public string Description { get; set; }
            public DateTime Date { get; set; }
            public decimal Amount { get; set; }
            public string Type { get; set; }
        }
    }
}