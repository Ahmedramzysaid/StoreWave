using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreWave.Services.Interfaces;
using StoreWave.ViewModels;

namespace StoreWave.Controllers
{
    [Authorize(Roles = "Accountant")]
    public class AccountantController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly ICustomerService _customerService;

        public AccountantController(
            IOrderService orderService,
            IProductService productService,
            ICustomerService customerService)
        {
            _orderService = orderService;
            _productService = productService;
            _customerService = customerService;
        }

        // GET: Accountant Dashboard - Financial Overview
        public async Task<IActionResult> Index()
        {
            var totalSales = await _orderService.GetTotalSalesAsync();
            var totalOrders = await _orderService.GetTotalOrdersAsync();
            var recentOrders = await _orderService.GetRecentOrdersAsync();

            var viewModel = new AccountantDashboardViewModel
            {
                TotalRevenue = totalSales,
                TotalOrders = totalOrders,
                AverageOrderValue = totalOrders > 0 ? totalSales / totalOrders : 0,
                RecentOrders = recentOrders.Take(10).ToList(),
                TodaysSales = recentOrders.Where(o => o.OrderDate.Date == DateTime.Today).Sum(o => o.TotalAmount),
                TodaysOrders = recentOrders.Count(o => o.OrderDate.Date == DateTime.Today),
                ThisMonthSales = recentOrders.Where(o => o.OrderDate.Month == DateTime.Today.Month && o.OrderDate.Year == DateTime.Today.Year).Sum(o => o.TotalAmount)
            };

            return View(viewModel);
        }

        // GET: Accountant/SalesReport
        public async Task<IActionResult> SalesReport(DateTime? startDate, DateTime? endDate)
        {
            var allOrders = await _orderService.GetRecentOrdersAsync();
            
            var start = startDate ?? DateTime.Today.AddMonths(-1);
            var end = endDate ?? DateTime.Today;

            var filteredOrders = allOrders
                .Where(o => o.OrderDate.Date >= start.Date && o.OrderDate.Date <= end.Date)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            ViewBag.StartDate = start;
            ViewBag.EndDate = end;
            ViewBag.TotalSales = filteredOrders.Sum(o => o.TotalAmount);
            ViewBag.OrderCount = filteredOrders.Count;

            return View(filteredOrders);
        }

        // GET: Accountant/OrderDetails/5
        public async Task<IActionResult> OrderDetails(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();

            return View(order);
        }

        // GET: Accountant/Revenue
        public async Task<IActionResult> Revenue()
        {
            var allOrders = await _orderService.GetRecentOrdersAsync();
            
            // Group by month for the last 12 months
            var monthlyRevenue = allOrders
                .Where(o => o.OrderDate >= DateTime.Today.AddMonths(-12))
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                .Select(g => new MonthlyRevenueViewModel
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Revenue = g.Sum(o => o.TotalAmount),
                    OrderCount = g.Count()
                })
                .OrderBy(m => m.Year)
                .ThenBy(m => m.Month)
                .ToList();

            return View(monthlyRevenue);
        }

        // GET: Accountant/PaymentMethods
        public async Task<IActionResult> PaymentMethods()
        {
            var allOrders = await _orderService.GetRecentOrdersAsync();
            
            var paymentStats = allOrders
                .GroupBy(o => o.PaymentMethod)
                .Select(g => new PaymentMethodStatsViewModel
                {
                    PaymentMethod = g.Key.ToString(),
                    OrderCount = g.Count(),
                    TotalAmount = g.Sum(o => o.TotalAmount)
                })
                .OrderByDescending(p => p.TotalAmount)
                .ToList();

            return View(paymentStats);
        }
    }
}
