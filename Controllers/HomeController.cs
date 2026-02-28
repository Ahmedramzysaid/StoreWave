using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using StoreWave.Services.Interfaces;
using StoreWave.ViewModels;
using System.Text.Json;

namespace StoreWave.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ILogger<HomeController> _logger;
        private readonly IDistributedCache _cache;

        private const string HomeViewModelCacheKey = "HomeViewModel";

        public HomeController(IProductService productService, ICategoryService categoryService, ILogger<HomeController> logger, IDistributedCache cache)
        {
            _productService = productService;
            _categoryService = categoryService;
            _logger = logger;
            _cache = cache;
        }

        public async Task<IActionResult> Index()
        {
            HomeViewModel? viewModel = null;

            try
            {
                var cachedData = await _cache.GetStringAsync(HomeViewModelCacheKey);
                if (!string.IsNullOrEmpty(cachedData))
                {
                    viewModel = JsonSerializer.Deserialize<HomeViewModel>(cachedData);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to read from distributed cache, fetching fresh data");
            }

            if (viewModel == null)
            {
                var featuredProducts = (await _productService.GetFeaturedProductsAsync())
                    .Where(p => !p.Name.Contains("Woman", StringComparison.OrdinalIgnoreCase) && 
                               !p.Name.Contains("Women", StringComparison.OrdinalIgnoreCase) &&
                               !p.Name.Contains("Girl", StringComparison.OrdinalIgnoreCase) &&
                               !p.Name.Contains("Lady", StringComparison.OrdinalIgnoreCase) &&
                               (p.CategoryName == null || (!p.CategoryName.Contains("Woman", StringComparison.OrdinalIgnoreCase) &&
                                                           !p.CategoryName.Contains("Women", StringComparison.OrdinalIgnoreCase))))
                    .Take(6)
                    .ToList();

                var onSaleProducts = await _productService.GetProductsOnSaleAsync();
                var categories = await _categoryService.GetActiveCategoriesAsync();

                viewModel = new HomeViewModel
                {
                    FeaturedProducts = featuredProducts,
                    OnSaleProducts = onSaleProducts.Take(4).ToList(),
                    Categories = categories.ToList()
                };

                try
                {
                    var cacheOptions = new DistributedCacheEntryOptions
                    {
                        SlidingExpiration = TimeSpan.FromMinutes(10),
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                    };

                    var serializedData = JsonSerializer.Serialize(viewModel);
                    await _cache.SetStringAsync(HomeViewModelCacheKey, serializedData, cacheOptions);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to write to distributed cache");
                }
            }

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}

