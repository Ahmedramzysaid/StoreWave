using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreWave.DTOs;
using StoreWave.Services.Interfaces;

namespace StoreWave.Controllers
{
    /// <summary>
    /// Controller for Category management (Admin only)
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return View(categories);
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View(new CategoryDto { IsActive = true });
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryDto categoryDto)
        {
            if (ModelState.IsValid)
            {
                var result = await _categoryService.CreateCategoryAsync(categoryDto);
                if (result)
                {
                    TempData["Success"] = "Category created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Failed to create category.");
            }
            return View(categoryDto);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryDto categoryDto)
        {
            if (id != categoryDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var result = await _categoryService.UpdateCategoryAsync(categoryDto);
                if (result)
                {
                    TempData["Success"] = "Category updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Failed to update category.");
            }
            return View(categoryDto);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);
            if (result)
            {
                TempData["Success"] = "Category deleted successfully.";
            }
            else
            {
                TempData["Error"] = "Failed to delete category. It may have associated products.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
