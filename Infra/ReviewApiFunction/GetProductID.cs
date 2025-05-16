
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.OpenApi.Models;
using System.Net;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using MerchStore.Application.DTOs;
using MerchStore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace MerchStore.Infra.ReviewApiFunction;

public class GetProductsID
{
    private readonly AppDbContext _db;


    public GetProductsID(AppDbContext db)
    {
        _db = db;

    }

    [Function("GetProductListForReview")]
    [OpenApiOperation("GetProductListForReview", tags: new[] { "ProductsID" })]
    [OpenApiResponseWithBody(
    statusCode: HttpStatusCode.OK,
    contentType: "application/json",
    bodyType: typeof(List<ProductForReviewDto>),
    Description = "Returns all product IDs and names")]
    public async Task<HttpResponseData> Run(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "products/simple")] HttpRequestData req, FunctionContext context)
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
        var products = await _db.Products
            .Select(p => new ProductForReviewDto
            {
                Id = p.Id.ToString(),
                Name = p.Name
            })
            .ToListAsync();

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(products);
        return response;
    }


}
