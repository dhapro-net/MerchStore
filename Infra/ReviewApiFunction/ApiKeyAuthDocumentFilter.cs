// filepath: MerchStoreDemo/infra/ReviewApiFunction/ApiKeyAuthDocumentFilter.cs
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.OpenApi.Models;

namespace ReviewApiFunction
{
    // Custom document filter to add API key authentication to the Swagger documentation
    public class ApiKeyAuthDocumentFilter : IDocumentFilter
    {
        public void Apply(IHttpRequestDataObject request, OpenApiDocument document)
        {
            // Initialize components if it's null
            if (document.Components == null)
            {
                document.Components = new OpenApiComponents();
            }

            if (document.Components.SecuritySchemes == null)
            {
                document.Components.SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>();
            }

            // Add API key security scheme
            document.Components.SecuritySchemes.Add("function_key", new OpenApiSecurityScheme
            {
                Name = "x-functions-key",
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header,
                Description = "Azure Function API Key authentication"
            });

            // Initialize security requirements if null
            document.SecurityRequirements ??= new List<OpenApiSecurityRequirement>();

            // Add global security requirement
            document.SecurityRequirements.Add(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "function_key"
                        }
                    },
                    new List<string>()
                }
            });
        }
    }
}