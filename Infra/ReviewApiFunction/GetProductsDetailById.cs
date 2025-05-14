
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using System.Net;
using MerchStore.Application.Services.Interfaces;
using MerchStore.Domain.Interfaces;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using MerchStore.Application.DTOs;
using MerchStore.Application.Services;
using MerchStore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace MerchStore.Infra.ReviewApiFunction;

public class GetProductsDetailById
{
    private readonly AppDbContext _db;


    public GetProductsDetailById(AppDbContext db)
    {
        _db = db;

    }

    [Function("GetProductDetailsById")]
    [OpenApiOperation("GetProductDetailsById", tags: new[] { "ProductsDetails" })]
    [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The product ID")]
    [OpenApiResponseWithBody(
    statusCode: HttpStatusCode.OK,
    contentType: "application/json",
    bodyType: typeof(ProductsdetailsDto),
    Description = "Returns full product details for one item")]
    [OpenApiResponseWithoutBody(HttpStatusCode.NotFound, Description = "Product not found")]
    [OpenApiResponseWithoutBody(HttpStatusCode.Unauthorized, Description = "Invalid API key")]
    [OpenApiResponseWithoutBody(HttpStatusCode.BadRequest, Description = "Invalid ID format")]
    public async Task<HttpResponseData> Run(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "products/{id}/details")] HttpRequestData req, string id, FunctionContext context)
    {
        var logger = context.GetLogger("AuthLog");

        // 🔐 Read API key from environment (from local.settings.json)
        string? expectedKey = Environment.GetEnvironmentVariable("ReviewApi:ApiKey");
        string headerName = Environment.GetEnvironmentVariable("ReviewApi:ApiKeyHeaderName") ?? "x-functions-key";

        if (!req.Headers.TryGetValues(headerName, out var apiKeys) ||
            apiKeys.FirstOrDefault() != expectedKey)
        {
            logger.LogWarning("Unauthorized access attempt.");
            var unauthorized = req.CreateResponse(HttpStatusCode.Unauthorized);
            await unauthorized.WriteStringAsync("Unauthorized: Missing or invalid API key.");
            return unauthorized;
        }
        if (!Guid.TryParse(id, out var productId))
        {
            var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequest.WriteStringAsync("Invalid product ID.");
            return badRequest;
        }

        var product = await _db.Products
            .Where(p => p.Id == productId)
            .Select(p => new ProductsdetailsDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price.Amount,
                Currency = p.Price.Currency.ToString(), // optional if needed
                ImageUrl = p.ImageUrl.ToString(),
                StockQuantity = p.StockQuantity
            })
            .FirstOrDefaultAsync();

        if (product == null)
        {
            var notFound = req.CreateResponse(HttpStatusCode.NotFound);
            await notFound.WriteStringAsync("Product not found.");
            return notFound;
        }

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(product);
        return response;
    }


}
