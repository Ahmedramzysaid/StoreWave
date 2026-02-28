using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreWave.Services.Interfaces;
using StoreWave.ViewModels;

namespace StoreWave.Controllers
{
    [Authorize(Roles = "WarehouseManager")]
    public class WarehouseController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public WarehouseController(
            IProductService productService,
            ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        // GET: Warehouse Dashboard
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();

            var viewModel = new WarehouseDashboardViewModel
            {
                TotalProducts = products.Count(),
                LowStockProducts = products.Where(p => p.StockQuantity > 0 && p.StockQuantity <= 10).ToList(),
                OutOfStockProducts = products.Where(p => p.StockQuantity == 0).ToList(),
                TotalStock = products.Sum(p => p.StockQuantity)
            };

            return View(viewModel);
        }

        // GET: Warehouse/Inventory
        public async Task<IActionResult> Inventory()
        {
            var products = await _productService.GetAllProductsAsync();
            return View(products.OrderBy(p => p.StockQuantity).ToList());
        }

        // GET: Warehouse/UpdateStock/5
        public async Task<IActionResult> UpdateStock(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();

            return View(product);
        }

        // POST: Warehouse/UpdateStock/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStock(int id, int stockQuantity)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();

            product.StockQuantity = stockQuantity;
            await _productService.UpdateProductAsync(product);

            TempData["Success"] = $"Stock updated for {product.Name}";
            return RedirectToAction(nameof(Inventory));
        }

        // GET: Warehouse/LowStock
        public async Task<IActionResult> LowStock()
        {
            var products = await _productService.GetAllProductsAsync();
            var lowStockProducts = products.Where(p => p.StockQuantity <= 10).OrderBy(p => p.StockQuantity).ToList();
            return View(lowStockProducts);
        }
    }
}
