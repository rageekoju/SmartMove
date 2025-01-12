using System;

namespace SmartMove.Models
{
    public class Dashboard
    {
        // Balance Section
        public decimal TotalIncome { get; set; } // Total inflows
        public decimal TotalExpense { get; set; } // Total outflows
        public decimal Balance => TotalIncome - TotalExpense; // Calculated balance

        // Debts Section
        public decimal TotalDebt { get; set; } // Total debts
        public decimal ClearedDebt { get; set; } // Cleared debts
        public decimal PendingDebt => TotalDebt - ClearedDebt; // Calculated pending debts

        // Date Filter
        public DateTime? FromDate { get; set; } // Start date for filters
        public DateTime? ToDate { get; set; } // End date for filters

        // Chart Data (Optional)
        public List<ChartData> ChartStatistics { get; set; } // Data for statistical charts

        // Constructor (Optional)
        public Dashboard()
        {
            ChartStatistics = new List<ChartData>();
        }
    }

    public class ChartData
    {
        public string Label { get; set; } // Label for the chart (e.g., "Income", "Expense")
        public decimal Value { get; set; } // Numeric value for the chart
    }
}
