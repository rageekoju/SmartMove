using System;

namespace SmartMove.Models
{
    public class Dashboard
    {
        public decimal TotalInflows { get; set; }
        public decimal TotalOutflows { get; set; }
        public decimal TotalDebts { get; set; }
        public decimal ClearedDebts { get; set; }
        public decimal PendingDebts { get; set; }
        public List<Transaction> TopTransactions { get; set; } = new();
        public bool IsSufficientBalanceForPendingDebts { get; set; }
    }
}