using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StoreWave.DTOs;
using StoreWave.Services.Interfaces;
using StoreWave.ViewModels;

namespace StoreWave.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IFileService _fileService;

        public ProductsController(IProductService productService, ICategoryService categoryService, IFileService fileService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _fileService = fileService;
        }

        public async Task<IActionResult> Index(int? categoryId, string? searchString)
        {
            IEnumerable<ProductDto> products;

            if (!string.IsNullOrEmpty(searchString))
            {
                products = await _productService.SearchProductsAsync(searchString);
            }
            else if (categoryId.HasValue)
            {
                products = await _productService.GetProductsByCategoryAsync(categoryId.Value);
            }
            else
            {
                products = await _productService.GetAllProductsAsync();
            }

            var categories = await _categoryService.GetActiveCategoriesAsync();
            var currentCategory = categoryId.HasValue ? categories.FirstOrDefault(c => c.Id == categoryId)?.Name : "All Products";

            var viewModel = new ProductListViewModel
            {
                Products = products,
                Categories = categories,
                CurrentCategoryId = categoryId,
                CurrentCategoryName = currentCategory,
                SearchString = searchString
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            ViewBag.CategoryId = new SelectList(await _categoryService.GetAllCategoriesAsync(), "Id", "Name");
            return View();
        }

        // POST: Products/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductDto productDto)
        {
            if (ModelState.IsValid)
            {
                if (productDto.ImageFile != null)
                {
                    productDto.ImageUrl = await _fileService.SaveFileAsync(productDto.ImageFile, "images/products");
                }

                await _productService.CreateProductAsync(productDto);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CategoryId = new SelectList(await _categoryService.GetAllCategoriesAsync(), "Id", "Name", productDto.CategoryId);
            return View(productDto);
        }

        // GET: Products/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewBag.CategoryId = new SelectList(await _categoryService.GetAllCategoriesAsync(), "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductDto productDto)
        {
            if (id != productDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (productDto.ImageFile != null)
                {
                     if (!string.IsNullOrEmpty(productDto.ImageUrl))
                     {
                         // Optional: Delete old file
                         // _fileService.DeleteFile(Path.GetFileName(productDto.ImageUrl), "images/products");
                     }
                     productDto.ImageUrl = await _fileService.SaveFileAsync(productDto.ImageFile, "images/products");
                }

                await _productService.UpdateProductAsync(productDto);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CategoryId = new SelectList(await _categoryService.GetAllCategoriesAsync(), "Id", "Name", productDto.CategoryId);
            return View(productDto);
        }

        // GET: Products/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productService.DeleteProductAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
