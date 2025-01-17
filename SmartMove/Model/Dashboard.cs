using System;
using System.Collections.Generic;

namespace SmartMove.Models
{
    // Represents the dashboard data that aggregates financial information
    public class Dashboard
    {
        // The total amount of money coming into the system (e.g., deposits, income)
        public decimal TotalInflows { get; set; }

        // The total amount of money going out of the system (e.g., expenses, purchases)
        public decimal TotalOutflows { get; set; }

        // The total outstanding debt, including both cleared and pending debts
        public decimal TotalDebts { get; set; }

        // The total amount of debt that has been cleared (paid off)
        public decimal ClearedDebts { get; set; }

        // The total amount of debt that is still pending to be paid
        public decimal PendingDebts { get; set; }

        // A list of the top transactions based on certain criteria, such as amount
        public List<Transaction> TopTransactions { get; set; } = new();

        // A flag indicating whether there is enough balance to cover the pending debts
        public bool IsSufficientBalanceForPendingDebts { get; set; }
    }
}
