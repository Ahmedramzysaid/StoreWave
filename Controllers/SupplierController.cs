using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StoreWave.DTOs;
using StoreWave.Models.Entities;
using StoreWave.Services.Interfaces;
using StoreWave.ViewModels;

namespace StoreWave.Controllers
{
    [Authorize(Roles = "Supplier")]
    public class SupplierController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IOrderService _orderService;
        private readonly IFileService _fileService;
        private readonly UserManager<Customer> _userManager;

        public SupplierController(
            IProductService productService,
            ICategoryService categoryService,
            IOrderService orderService,
            IFileService fileService,
            UserManager<Customer> userManager)
        {
            _productService = productService;
            _categoryService = categoryService;
            _orderService = orderService;
            _fileService = fileService;
            _userManager = userManager;
        }

        // GET: Supplier Dashboard
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var allProducts = await _productService.GetAllProductsAsync();
            var myProducts = allProducts.Where(p => p.SupplierId == user.Id).ToList();

            var viewModel = new SupplierDashboardViewModel
            {
                TotalProducts = myProducts.Count,
                ActiveProducts = myProducts.Count(p => p.IsActive),
                OutOfStockProducts = myProducts.Count(p => p.StockQuantity == 0),
                RecentProducts = myProducts.OrderByDescending(p => p.Id).Take(5).ToList()
            };

            return View(viewModel);
        }

        // GET: Supplier/Products
        public async Task<IActionResult> Products()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var allProducts = await _productService.GetAllProductsAsync();
            var myProducts = allProducts.Where(p => p.SupplierId == user.Id).ToList();

            return View(myProducts);
        }

        // GET: Supplier/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _categoryService.GetActiveCategoriesAsync();
            return View();
        }

        // POST: Supplier/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductDto productDto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                // Handle image upload
                if (productDto.ImageFile != null)
                {
                    var imageUrl = await _fileService.SaveFileAsync(productDto.ImageFile, "products");
                    productDto.ImageUrl = imageUrl;
                }

                productDto.SupplierId = user.Id;
                await _productService.CreateProductAsync(productDto);
                TempData["Success"] = "Product created successfully!";
                return RedirectToAction(nameof(Products));
            }

            ViewBag.Categories = await _categoryService.GetActiveCategoriesAsync();
            return View(productDto);
        }

        // GET: Supplier/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var product = await _productService.GetProductByIdAsync(id);
            if (product == null || product.SupplierId != user.Id)
            {
                return NotFound();
            }

            ViewBag.Categories = await _categoryService.GetActiveCategoriesAsync();
            return View(product);
        }

        // POST: Supplier/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductDto productDto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            if (id != productDto.Id) return BadRequest();

            // Verify ownership
            var existingProduct = await _productService.GetProductByIdAsync(id);
            if (existingProduct == null || existingProduct.SupplierId != user.Id)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                // Handle image upload
                if (productDto.ImageFile != null)
                {
                    var imageUrl = await _fileService.SaveFileAsync(productDto.ImageFile, "products");
                    productDto.ImageUrl = imageUrl;
                }
                else
                {
                    productDto.ImageUrl = existingProduct.ImageUrl;
                }

                productDto.SupplierId = user.Id;
                await _productService.UpdateProductAsync(productDto);
                TempData["Success"] = "Product updated successfully!";
                return RedirectToAction(nameof(Products));
            }

            ViewBag.Categories = await _categoryService.GetActiveCategoriesAsync();
            return View(productDto);
        }

        // POST: Supplier/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var product = await _productService.GetProductByIdAsync(id);
            if (product == null || product.SupplierId != user.Id)
            {
                return Forbid();
            }

            await _productService.DeleteProductAsync(id);
            TempData["Success"] = "Product deleted successfully!";
            return RedirectToAction(nameof(Products));
        }

        // GET: Supplier/Orders - Orders containing supplier's products
        // GET: Supplier/Orders - Orders containing supplier's products
        public async Task<IActionResult> Orders()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var allProducts = await _productService.GetAllProductsAsync();
            var myProductIds = allProducts.Where(p => p.SupplierId == user.Id).Select(p => p.Id).ToList();

            if (!myProductIds.Any())
            {
                return View(new List<OrderDto>());
            }

            var recentOrders = await _orderService.GetRecentOrdersAsync();
            
            // Filter orders that contain at least one product from this supplier
            var myOrders = recentOrders
                .Where(o => o.OrderItems.Any(i => myProductIds.Contains(i.ProductId)))
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return View(myOrders);
        }
    }
}
