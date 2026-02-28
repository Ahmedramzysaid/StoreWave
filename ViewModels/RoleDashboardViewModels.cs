using StoreWave.DTOs;

namespace StoreWave.ViewModels
{
    /// <summary>
    /// Dashboard ViewModel for Suppliers
    /// </summary>
    public class SupplierDashboardViewModel
    {
        public int TotalProducts { get; set; }
        public int ActiveProducts { get; set; }
        public int OutOfStockProducts { get; set; }
        public decimal TotalSales { get; set; }
        public List<ProductDto> RecentProducts { get; set; } = new();
    }

    /// <summary>
    /// Dashboard ViewModel for Warehouse Managers
    /// </summary>
    public class WarehouseDashboardViewModel
    {
        public int TotalProducts { get; set; }
        public int TotalStock { get; set; }
        public List<ProductDto> LowStockProducts { get; set; } = new();
        public List<ProductDto> OutOfStockProducts { get; set; } = new();
    }

    /// <summary>
    /// Dashboard ViewModel for Accountants
    /// </summary>
    public class AccountantDashboardViewModel
    {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public decimal TodaysSales { get; set; }
        public int TodaysOrders { get; set; }
        public decimal ThisMonthSales { get; set; }
        public List<OrderDto> RecentOrders { get; set; } = new();
    }

    /// <summary>
    /// Monthly revenue data for reports
    /// </summary>
    public class MonthlyRevenueViewModel
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
        
        public string MonthName => new DateTime(Year, Month, 1).ToString("MMM yyyy");
    }

    /// <summary>
    /// Payment method statistics for reports
    /// </summary>
    public class PaymentMethodStatsViewModel
    {
        public string PaymentMethod { get; set; } = string.Empty;
        public int OrderCount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
