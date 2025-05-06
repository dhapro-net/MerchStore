using Microsoft.AspNetCore.Mvc;
using MerchStore.Application.Services.Interfaces;
using MerchStore.WebUI.Models;

namespace MerchStore.WebUI.Controllers;

public class ReviewsController : Controller
{
    private readonly IReviewService _reviewService;
    private readonly ICatalogService _catalogService;

    public ReviewsController(IReviewService reviewService, ICatalogService catalogService)
    {
        _reviewService = reviewService;
        _catalogService = catalogService;
    }

    // GET: Reviews
    public async Task<IActionResult> Index()
    {
        try
        {
            // Get all products
            var products = await _catalogService.GetAllProductsAsync();
            var viewModel = new ProductReviewsViewModel
            {
                Products = products.ToList()
            };

            // For each product, get its reviews and calculate the average rating
            foreach (var product in viewModel.Products)
            {
                viewModel.ProductReviews[product.Id] = await _reviewService.GetReviewsByProductIdAsync(product.Id);
                viewModel.AverageRatings[product.Id] = await _reviewService.GetAverageRatingForProductAsync(product.Id);
                viewModel.ReviewCounts[product.Id] = await _reviewService.GetReviewCountForProductAsync(product.Id);
            }

            return View(viewModel);
        }
        catch (Exception ex)
        {
            // Log the error and return an error view
            TempData["ErrorMessage"] = $"Error fetching reviews: {ex.Message}";
            return View("Error");
        }
    }

    // GET: Reviews/Product/{id}
    public async Task<IActionResult> Product(Guid id)
    {
        try
        {
            // Get the product by ID
            var product = await _catalogService.GetProductByIdAsync(id);

            if (product is null)
            {
                return NotFound();
            }

            // Get reviews for the product
            var reviews = await _reviewService.GetReviewsByProductIdAsync(id);
            var averageRating = await _reviewService.GetAverageRatingForProductAsync(id);
            var reviewCount = await _reviewService.GetReviewCountForProductAsync(id);

            var viewModel = new ProductReviewViewModel
            {
                Product = product,
                Reviews = reviews.ToList(),
                AverageRating = averageRating,
                ReviewCount = reviewCount
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            // Log the error and return an error view
            TempData["ErrorMessage"] = $"Error fetching product reviews: {ex.Message}";
            return View("Error");
        }
    }
}