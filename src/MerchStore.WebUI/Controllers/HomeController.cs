using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MerchStore.WebUI.Models;
using MerchStore.WebUI.Models.Home;
using MerchStore.WebUI.Models.Catalog;
using MerchStore.Application.Services.Interfaces;
using MerchStore.WebUI.Helpers;

namespace MerchStore.WebUI.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IReviewService _reviewService;
    private readonly ICatalogService _catalogService;


    public HomeController(ILogger<HomeController> logger, IReviewService reviewService, ICatalogService catalogService)
    {
        _logger = logger;
        _reviewService = reviewService;
        _catalogService = catalogService;
    }



    public async Task<IActionResult> Index()
    {
        var products = await _catalogService.GetAllProductsAsync();
        // Get the top 3 products based on some criteria (e.g., best-selling, highest rated, etc.)
        // For simplicity, let's just take the first 3 products
        var topProducts = products.Take(3).ToList();

        var productCards = new List<ProductCardViewModel>();

        foreach (var product in topProducts)
        {
            var averageRating = await _reviewService.GetAverageRatingForProductAsync(product.Id);
            var reviewCount = await _reviewService.GetReviewCountForProductAsync(product.Id);

            productCards.Add(new ProductCardViewModel
            {
                Id = product.Id,
                Name = product.Name,
                FormattedPrice = product.Price.Amount.ToString("0 kr"),
                PriceAmount = product.Price.Amount,
                ImageUrl = product.ImageUrl?.ToString(),
                HoverImageUrl = HoverImageHelper.GetHoverImageUrl(product.Name),
                StockQuantity = product.StockQuantity,
                AverageRating = averageRating,
                ReviewCount = reviewCount
            });
        }

        // Populate hero slides
        var viewModel = new HomePageViewModel
        {
            // Populate hero slides
            HeroSlides = new List<HeroSlideViewModel>
                {
                    new HeroSlideViewModel
                    {
                        ImageUrl = "/img/canvas02.png",
                        Heading = "Premium Headphones",
                        Description = "Experience crystal clear sound with our latest wireless headphones. Perfect for music lovers and professionals alike.",
                        ButtonText = "Learn More",
                        ButtonUrl = "#"
                    },
                    new HeroSlideViewModel
                    {
                        ImageUrl = "/img/hoodie03.jpg",
                        Heading = "Smart Watch",
                        Description = "Stay connected and track your fitness with our feature-rich smartwatch. Water-resistant and long battery life.",
                        ButtonText = "Learn More",
                        ButtonUrl = "#"
                    },
                    new HeroSlideViewModel
                    {
                        ImageUrl = "/img/coaster01.png",
                        Heading = "Wireless Earbuds",
                        Description = "Compact and powerful wireless earbuds with noise cancellation. Perfect for your daily commute.",
                        ButtonText = "Learn More",
                        ButtonUrl = "#"
                    }
                },

             PopularProducts = productCards,

            // Newsletter section
            Newsletter = new NewsletterViewModel
            {
                Heading = "Nail your first purchase with 15% off",
                Subheading = "By signing up to receive latest product drops, discounts and more from Wella brands straight to your inbox."
            },

            // Shipping features
            ShippingFeatures = new List<ShippingFeatureViewModel>
                {
                    new ShippingFeatureViewModel
                    {
                        Icon = "fa-truck",
                        Title = "Free Shipping",
                        Description = "from 1000 kr of purchase"
                    },
                    new ShippingFeatureViewModel
                    {
                        Icon = "fa-store",
                        Title = "Come meet us in our store",
                        Description = ""
                    },
                    new ShippingFeatureViewModel
                    {
                        Icon = "fa-undo",
                        Title = "Easy returns",
                        Description = "Return within 30 days for a full refund, no questions asked."
                    }
                }
        };

        return View(viewModel);
    }



    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
