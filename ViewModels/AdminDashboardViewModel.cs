using StoreWave.DTOs;

namespace StoreWave.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalOrders { get; set; }
        public decimal TotalSales { get; set; }
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int TotalCustomers { get; set; }
        public int PendingOrders { get; set; }
        public List<OrderDto> RecentOrders { get; set; } = new();
    }
}
