using MerchStore.Application.Services.Interfaces;
using MerchStore.WebUI.Models.Api.Minimal;

namespace MerchStore.WebUI.Endpoints;


public static class MinimalProductEndpoints
{
 
    public static WebApplication MapMinimalProductEndpoints(this WebApplication app)
    {
        // Define a route group for the minimal product API
        var group = app.MapGroup("/api/minimal/products")
            .WithTags("Minimal Products API")
            .WithOpenApi();

        // Get all products endpoint
        group.MapGet("/", GetAllProducts)
            .WithName("GetAllProductsMinimal")
            .WithDescription("Gets all available products")
            .Produces<List<MinimalProductResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);

        // Get product by ID endpoint
        group.MapGet("/{id}", GetProductById)
            .WithName("GetProductByIdMinimal")
            .WithDescription("Gets a specific product by its unique identifier")
            .Produces<MinimalProductResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        return app;
    }


    private static async Task<IResult> GetAllProducts(ICatalogService catalogService, CancellationToken cancellationToken)
    {
        try
        {
            // Get all products from the service
            var products = await catalogService.GetAllProductsAsync(cancellationToken);

            // Map domain entities to response objects
            var response = products.Select(p => new MinimalProductResponse
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price.Amount,
                Currency = p.Price.Currency,
                ImageUrl = p.ImageUrl?.ToString(),
                StockQuantity = p.StockQuantity
            }).ToList();

            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            // Log the exception in a real application
            return Results.Problem($"An error occurred while retrieving products: {ex.Message}");
        }
    }

    private static async Task<IResult> GetProductById(Guid id, ICatalogService catalogService, CancellationToken cancellationToken)
    {
        try
        {
            // Get the specific product from the service
            var product = await catalogService.GetProductByIdAsync(id, cancellationToken);

            // Return 404 Not Found if the product doesn't exist
            if (product is null)
            {
                return Results.NotFound($"Product with ID {id} not found");
            }

            // Map domain entity to response object
            var response = new MinimalProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price.Amount,
                Currency = product.Price.Currency,
                ImageUrl = product.ImageUrl?.ToString(),
                StockQuantity = product.StockQuantity
            };

            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            // Log the exception in a real application
            return Results.Problem($"An error occurred while retrieving the product: {ex.Message}");
        }
    }
}