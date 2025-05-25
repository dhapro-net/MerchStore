using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MerchStore.WebUI.Models;
using MerchStore.Application.Services.Interfaces;
using MerchStore.WebUI.Models.Dashboard;
using MerchStore.Domain.Entities;
using MerchStore.Domain.Interfaces;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using MerchStore.Application.Common.Interfaces;
using MerchStore.Domain.ValueObjects;
using DnsClient.Internal;
using MerchStore.Application.DTOs;

namespace MerchStore.WebUI.Controllers;



[Authorize]
public class AdminController : Controller
{
    private readonly ICatalogService _catalogService;
    private readonly IOrderService _orderService;

    public AdminController(ICatalogService catalogService, IOrderService orderService)
    {
        _catalogService = catalogService;
         _orderService = orderService;
    }

    public IActionResult Dashboard()
    {
        return View();
    }

    // GET: Admin/Products
    public async Task<IActionResult> Products(string searchTerm)
    {
        IEnumerable<Domain.Entities.Product> products;

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            products = await _catalogService.SearchProductsAsync(searchTerm);
            ViewBag.SearchTerm = searchTerm;
        }
        else
        {
            products = await _catalogService.GetAllProductsAsync(HttpContext.RequestAborted);
        }

        // Map to view models
        var viewModels = products.Select(p => new ProductViewModel
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price.Amount,
            Stock = p.StockQuantity,
            ImageUrl = p.ImageUrl?.ToString(),
            Currency = "SEK"
        }).ToList();

        return View(viewModels);
    }

    // GET: Admin/CreateProduct
    public IActionResult CreateProduct()
    {
        return View(new ProductViewModel());
    }

    // POST: Admin/CreateProduct
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateProduct(ProductViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            // Parse image URL
            Uri? imageUri = null;
            if (!string.IsNullOrEmpty(model.ImageUrl))
            {
                Uri.TryCreate(model.ImageUrl, UriKind.Absolute, out imageUri);
            }

            // Use the service to add the product
            await _catalogService.AddProductAsync(
                name: model.Name,
                description: model.Description,
                imageUrl: imageUri,
                price: model.Price,
                stockQuantity: model.Stock
            );

            TempData["SuccessMessage"] = "Product created successfully!";
            return RedirectToAction(nameof(Products));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    // GET: Admin/EditProduct/{id}
    public async Task<IActionResult> EditProduct(Guid id)
    {
        var product = await _catalogService.GetProductByIdAsync(id, HttpContext.RequestAborted);
        if (product == null)
        {
            return NotFound();
        }

        var viewModel = new ProductViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price.Amount,
            Stock = product.StockQuantity,
            ImageUrl = product.ImageUrl?.ToString(),
            Currency = "SEK"
        };

        return View(viewModel);
    }

    // POST: Admin/EditProduct
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProduct(ProductViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            // Parse image URL
            Uri? imageUri = null;
            if (!string.IsNullOrEmpty(model.ImageUrl))
            {
                Uri.TryCreate(model.ImageUrl, UriKind.Absolute, out imageUri);
            }

            // Use the service to update the product
            await _catalogService.UpdateProductAsync(
                id: model.Id,
                name: model.Name,
                description: model.Description,
                imageUrl: imageUri,
                price: model.Price,
                stockQuantity: model.Stock
            );

            TempData["SuccessMessage"] = "Product updated successfully!";
            return RedirectToAction(nameof(Products));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating product: {ex.Message}");
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    // POST: Admin/DeleteProduct/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        try
        {
            await _catalogService.DeleteProductAsync(id);

            TempData["SuccessMessage"] = "Product deleted successfully!";
            return RedirectToAction(nameof(Products));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting product: {ex.Message}");
            TempData["ErrorMessage"] = $"Error deleting product: {ex.Message}";
            return RedirectToAction(nameof(Products));
        }
    }
    //order management
    public IActionResult Orders()
    {
        // Create some sample data for testing
        var sampleOrders = new List<OrderViewModel>
    {
        new OrderViewModel
        {
            Id = Guid.Parse("6b9e3551-0000-0000-0000-000000000000"),
            CustomerName = "John Doe",
            CustomerEmail = "john.doe@example.com",
            Status = "Delivered",
            ItemCount = 2,
            Total = 649.48M,
            Currency = "SEK"
        },
        new OrderViewModel
        {
            Id = Guid.Parse("c2ebd4fe-0000-0000-0000-000000000000"),
            CustomerName = "Jane Smith",
            CustomerEmail = "jane.smith@example.com",
            Status = "Processed",
            ItemCount = 2,
            Total = 739.96M,
            Currency = "SEK"
        },
        new OrderViewModel
        {
            Id = Guid.Parse("07159d4e-0000-0000-0000-000000000000"),
            CustomerName = "Bob Johnson",
            CustomerEmail = "bob.johnson@example.com",
            Status = "Created",
            ItemCount = 3,
            Total = 878.98M,
            Currency = "SEK"
        }
    };

        return View(sampleOrders);
    }
    // GET: /Admin/CreateOrder
    [HttpGet]
    public IActionResult CreateOrder()
    {
        return View(new CreateOrderViewModel
        {
            CustomerName = string.Empty,
            CustomerEmail = string.Empty,
            StreetAddress = string.Empty,
            City = string.Empty,
            ZipCode = string.Empty,
            Country = string.Empty
        });
    }

    // POST: /Admin/CreateOrder
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateOrder(CreateOrderViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var dto = new OrderProductDto
        {
            CustomerName = model.CustomerName,
            CustomerEmail = model.CustomerEmail,
            StreetAddress = model.StreetAddress,
            City = model.City,
            ZipCode = model.ZipCode,
            Country = model.Country
        };

        await _orderService.CreateAsync(dto);

        TempData["Success"] = "✅ Order created successfully!";
        return RedirectToAction("Orders");
    }

}

