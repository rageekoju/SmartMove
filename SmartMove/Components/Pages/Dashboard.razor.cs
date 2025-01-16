namespace SmartMove.Components.Pages
{
    public partial class Dashboard
    {
        private decimal TotalInflows;
        private decimal TotalOutflows;
        private decimal TotalDebt;
        private decimal ClearedDebt;
        private decimal PendingDebt;
        private decimal AvailableBalance;
        private List<Transaction> TopTransactions = new();
        private List<Transaction> PendingDebts = new();
        private DateTime? StartDate;
        private DateTime? EndDate;

        protected override async Task OnInitializedAsync()
        {
            await LoadDashboardData();
        }

        private async Task LoadDashboardData()
        {
            var dashboardData = await TransactionService.GetDashboardDataAsync();
            TotalInflows = dashboardData["Total Inflows"];
            TotalOutflows = dashboardData["Total Outflows"];
            ClearedDebt = dashboardData["Cleared Debt"];
            PendingDebt = dashboardData["Pending Debt"];
            TotalDebt = dashboardData["Total Debt"];
            AvailableBalance = TotalInflows - TotalOutflows - ClearedDebt;

            TopTransactions = await TransactionService.GetTopTransactionsAsync(true);
            PendingDebts = (await TransactionService.GetAllTransactionsAsync()).Where(t => t.Type == "Pending Debt").ToList();
        }

        private async Task FilterTransactions()
        {
            if (StartDate.HasValue && EndDate.HasValue)
            {
                var filteredTransactions = await TransactionService.GetFilteredTransactionsAsync(StartDate.Value, EndDate.Value);
                TopTransactions = filteredTransactions.OrderByDescending(t => t.Amount).Take(5).ToList();
            }
        }

        private int Index = -1; //default value cannot be 0 -> first selectedindex is 0.
        int dataSize = 5;
        double[] data = { 40, 30, 15, 10, 5 }; // These represent Inflows, Outflows, Debt, Cleared Debt, and Pending Debt
        string[] labels = { "Inflows", "Outflows", "Debt", "Cleared Debt", "Pending Debt" }; // Labels corresponding to the categories
        string[] colors = { "#cb997e", "#ddbea9", "#ffe8d6", "#b7b7a4", "#a5a58d" }; // Colors for each category

        Random random = new Random();

        void RandomizeData()
        {
            var new_data = new double[dataSize];
            for (int i = 0; i < new_data.Length; i++)
                new_data[i] = random.NextDouble() * 100;
            data = new_data;
            StateHasChanged();
        }

        void AddDataSize()
        {
            if (dataSize < 20)
            {
                dataSize = dataSize + 1;
                RandomizeData();
            }
        }
        void RemoveDataSize()
        {
            if (dataSize > 0)
            {
                dataSize = dataSize - 1;
                RandomizeData();
            }
        }
    }
}
