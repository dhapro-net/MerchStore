using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MerchStore.WebUI.Models;
using MerchStore.WebUI.Models.Home;
using MerchStore.WebUI.Models.Catalog;
using MerchStore.Application.Services.Interfaces;
using MerchStore.WebUI.Helpers;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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



    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var products = await _catalogService.GetAllProductsAsync(cancellationToken);

        var canvas = products.FirstOrDefault(p => p.Name.Contains("Canvas"));
        var hoodie = products.FirstOrDefault(p => p.Name.Contains("Hoodie"));
        var coaster = products.FirstOrDefault(p => p.Name.Contains("Coaster"));
        // Get the top 3 products based on some criteria (e.g., best-selling, highest rated, etc.)
        // For simplicity, let's just take the first 3 products
        var topProducts = products.Take(6).ToList();

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
        Heading = "20% Off Canvas Prints!",
        Description = "Brighten your space with our bold and artistic canvas designs. This week only!",
        ButtonText = "Shop Canvas",
        ButtonUrl = canvas != null ? Url.Action("Details", "Catalog", new { id = canvas.Id }) : "#"
    },
    new HeroSlideViewModel
    {
        ImageUrl = "/img/hoodie03.jpg",
        Heading = "Comfy Hoodies Are Here",
        Description = "Get cozy in style – limited edition colors just dropped.",
        ButtonText = "Explore Hoodies",
        ButtonUrl = hoodie != null ? Url.Action("Details", "Catalog", new { id = hoodie.Id }) : "#"
    },
    new HeroSlideViewModel
    {
        ImageUrl = "/img/coaster01.png",
        Heading = "Sip in Style ☕",
        Description = "Check out our coffee coasters – a small touch with big personality.",
        ButtonText = "Shop Coasters",
        ButtonUrl = coaster != null ? Url.Action("Details", "Catalog", new { id = coaster.Id }) : "#"
    }
},

            PopularProducts = productCards,

            // Newsletter section
            Newsletter = new NewsletterViewModel
            {
                Heading = "For your first purchase with 15% off",
                Subheading = "Sign up to get exclusive deals, new arrivals, and special offers delivered to your inbox."
            },

            // Shipping features
            ShippingFeatures = new List<ShippingFeatureViewModel>
                {
                    new ShippingFeatureViewModel
                    {
                        Icon = "bi bi-truck",
                        Title = "Free Shipping",
                        Description = "from 1000 kr of purchase"
                    },
                    new ShippingFeatureViewModel
                    {
                        Icon = "bi bi-shop",
                        Title = "Come meet us in our store",
                        Description = ""
                    },
                    new ShippingFeatureViewModel
                    {
                        Icon = "bi bi-arrow-counterclockwise",
                        Title = "Easy returns",
                        Description = "Return within 30 days for a full refund."
                    }
                }
        };

        return View(viewModel);
    }



    public IActionResult Privacy()
    {
        return View();
    }
    public IActionResult About()
    {
        return View(); // Looks for Views/Home/About.cshtml
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
