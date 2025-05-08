using Microsoft.AspNetCore.Authentication;

namespace MerchStore.WebUI.Authentication.ApiKey;


// Options for API key authentication.

public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    
    public string HeaderName { get; set; } = ApiKeyAuthenticationDefaults.HeaderName;

    public string ApiKey { get; set; } = string.Empty;
}