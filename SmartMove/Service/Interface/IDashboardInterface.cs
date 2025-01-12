using SmartMove.Models;

namespace SmartMove.Interfaces
{
    public interface IDashboardInterface
    {
        Task<Dashboard> GetDashboardDataAsync(DateTime? fromDate, DateTime? toDate);
    }
}
