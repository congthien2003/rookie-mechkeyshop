namespace Shared.ViewModels.Dashboard
{
    public class DashboardData
    {
        public decimal TotalRevenue { get; set; }
        public int TotalSellCount { get; set; }
        public int TotalOrderCount { get; set; }
        public int TotalOrderCompleted { get; set; }
        public int TotalOrderPending { get; set; }
        public int TotalProductAvailable { get; set; }
        public int TotalUserAvailable { get; set; }
    }
}