namespace SmartMove.Components.Pages
{
    public partial class Dashboard
    {
        // Financial summary variables
        private decimal TotalInflows;
        private decimal TotalOutflows;
        private decimal TotalDebt;
        private decimal ClearedDebt;
        private decimal PendingDebt;
        private decimal AvailableBalance;

        // Lists to store transaction data
        private List<Transaction> TopTransactions = new();
        private List<Transaction> PendingDebts = new();

        // Date range for filtering transactions
        private DateTime? StartDate;
        private DateTime? EndDate;

        // Lifecycle method to initialize data when the component loads
        protected override async Task OnInitializedAsync()
        {
            await LoadDashboardData();
        }

        // Method to load dashboard financial data and transaction lists
        private async Task LoadDashboardData()
        {
            var dashboardData = await TransactionService.GetDashboardDataAsync();

            // Assigning values from retrieved dashboard data
            TotalInflows = dashboardData["Total Inflows"];
            TotalOutflows = dashboardData["Total Outflows"];
            ClearedDebt = dashboardData["Cleared Debt"];
            PendingDebt = dashboardData["Pending Debt"];
            TotalDebt = dashboardData["Total Debt"];
            AvailableBalance = TotalInflows - TotalOutflows - ClearedDebt;

            // Fetching transactions
            TopTransactions = await TransactionService.GetTopTransactionsAsync(true);
            PendingDebts = (await TransactionService.GetAllTransactionsAsync())
                            .Where(t => t.Type == "Pending Debt").ToList();
        }

        // Filters transactions based on the selected date range
        private async Task FilterTransactions()
        {
            if (StartDate.HasValue && EndDate.HasValue)
            {
                var filteredTransactions = await TransactionService.GetFilteredTransactionsAsync(StartDate.Value, EndDate.Value);
                TopTransactions = filteredTransactions.OrderByDescending(t => t.Amount).Take(5).ToList();
            }
        }

        // Index for managing selected data points
        private int Index = -1; // Default value is -1 as 0 represents the first selected index

        // Data representation for financial distribution
        int dataSize = 5;
        double[] data = { 40, 30, 15, 10, 5 }; // Representing Inflows, Outflows, Debt, Cleared Debt, and Pending Debt
        string[] labels = { "Inflows", "Outflows", "Debt", "Cleared Debt", "Pending Debt" }; // Labels for the dataset
        string[] colors = { "#cb997e", "#ddbea9", "#ffe8d6", "#b7b7a4", "#a5a58d" }; // Corresponding colors for visualization

        // Random instance for generating dynamic values
        Random random = new Random();

        // Generates random values for the dataset
        void RandomizeData()
        {
            var new_data = new double[dataSize];
            for (int i = 0; i < new_data.Length; i++)
                new_data[i] = random.NextDouble() * 100;
            data = new_data;
            StateHasChanged(); // Refresh UI with new data
        }

        // Increases the data size up to a maximum limit
        void AddDataSize()
        {
            if (dataSize < 20)
            {
                dataSize = dataSize + 1;
                RandomizeData();
            }
        }

        // Decreases the data size while ensuring it does not go below zero
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
