using SmartMove.Interfaces;
using SmartMove.Models;

namespace SmartMove.Services
{
    public class DashboardService : IDashboardInterface
    {
        // Example: Simulating a data source (e.g., database context)
        private readonly List<Transaction> _transactions;
        private readonly List<Debt> _debts;

        public DashboardService()
        {
            // Initialize with dummy data for now
            _transactions = new List<Transaction>
            {
                new Transaction { Amount = 100, Type = "Income", Date = DateTime.Now.AddDays(-5) },
                new Transaction { Amount = 50, Type = "Expense", Date = DateTime.Now.AddDays(-2) }
            };

            _debts = new List<Debt>
            {
                new Debt { Amount = 300, IsCleared = false },
                new Debt { Amount = 100, IsCleared = true }
            };
        }

        public async Task<Dashboard> GetDashboardDataAsync(DateTime? fromDate, DateTime? toDate)
        {
            // Filter transactions based on date range
            var filteredTransactions = _transactions
                .Where(t => (!fromDate.HasValue || t.Date >= fromDate) &&
                            (!toDate.HasValue || t.Date <= toDate));

            // Calculate income and expense
            var totalIncome = filteredTransactions.Where(t => t.Type == "Income").Sum(t => t.Amount);
            var totalExpense = filteredTransactions.Where(t => t.Type == "Expense").Sum(t => t.Amount);

            // Calculate debt statistics
            var totalDebt = _debts.Sum(d => d.Amount);
            var clearedDebt = _debts.Where(d => d.IsCleared).Sum(d => d.Amount);

            // Return the dashboard model
            var dashboard = new Dashboard
            {
                TotalIncome = totalIncome,
                TotalExpense = totalExpense,
                TotalDebt = totalDebt,
                ClearedDebt = clearedDebt,
                FromDate = fromDate,
                ToDate = toDate,
                ChartStatistics = new List<ChartData>
                {
                    new ChartData { Label = "Income", Value = totalIncome },
                    new ChartData { Label = "Expense", Value = totalExpense },
                    new ChartData { Label = "Debt", Value = totalDebt }
                }
            };

            return await Task.FromResult(dashboard); // Simulating async operation
        }
    }

    // Example: Transaction model
    public class Transaction
    {
        public decimal Amount { get; set; }
        public string Type { get; set; } // "Income" or "Expense"
        public DateTime Date { get; set; }
    }

    // Example: Debt model
    public class Debt
    {
        public decimal Amount { get; set; }
        public bool IsCleared { get; set; }
    }
}
