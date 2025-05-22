using MerchStore.Application;
using MerchStore.Application.Services.Implementations;
using MerchStore.Application.Services.Interfaces;
using MerchStore.Infrastructure;
using MerchStore.WebUI.Authentication.ApiKey;
using MerchStore.WebUI.Endpoints;
using MerchStore.WebUI.Infrastructure;
using MerchStore.WebUI.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text.Json.Serialization;





var builder = WebApplication.CreateBuilder(args);


builder.Services.AddScoped<ICatalogService, CatalogService>();


// Update the JSON options configuration to use our custom policy
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        // Use snake_case for JSON serialization
        options.JsonSerializerOptions.PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy();
        options.JsonSerializerOptions.DictionaryKeyPolicy = new JsonSnakeCaseNamingPolicy();
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });


// Add Razor Pages (required for Identity UI)
builder.Services.AddRazorPages();

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

// Add API Key authentication
builder.Services.AddAuthentication()
   .AddApiKey(builder.Configuration["ApiKey:Value"] ?? throw new InvalidOperationException("API Key is not configured in the application settings."));

// Add API Key authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiKeyPolicy", policy =>
        policy.AddAuthenticationSchemes(ApiKeyAuthenticationDefaults.AuthenticationScheme)
              .RequireAuthenticatedUser());
});

// Add Application services - this includes Services, Interfaces, etc.
builder.Services.AddApplication();

// Add Infrastructure services - this includes DbContext, Repositories, etc.
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<CookieShoppingCartService>();

builder.Services.AddHttpContextAccessor();

// Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MerchStore API",
        Version = "v1",
        Description = "API for MerchStore product catalog",
        Contact = new OpenApiContact
        {
            Name = "MerchStore Support",
            Email = "support@merchstore.example.com"
        }
    });

    // Include XML comments if you've enabled XML documentation in your project
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
    // Configure operation IDs for minimal APIs to avoid conflicts
    options.CustomOperationIds(apiDesc =>
    {
        return apiDesc.TryGetMethodInfo(out var methodInfo) ? methodInfo.Name : null;
    });
    // Add API Key authentication support to Swagger UI
    options.AddSecurityDefinition(ApiKeyAuthenticationDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Description = "API Key Authentication. Enter your API key in the field below.",
        Name = ApiKeyAuthenticationDefaults.HeaderName,
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = ApiKeyAuthenticationDefaults.AuthenticationScheme
    });

    // Apply API key requirement only to controller-based endpoints
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});


// Add this after other service registrations
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()  // Allow requests from any origin
                   .AllowAnyHeader()  // Allow any headers
                   .AllowAnyMethod(); // Allow any HTTP method
        });
});

builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
});

// Configure authentication with multiple schemes
var authBuilder = builder.Services.AddAuthentication(options =>
{
    // Set the default scheme to check all authentication types
    options.DefaultScheme = "CustomMultiAuthScheme"; // Default scheme for authentication
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme; // Default challenge to standard cookie login
})
    .AddPolicyScheme("CustomMultiAuthScheme", "Custom MultiAuth Scheme", options =>
    {
        // This policy scheme will check the name of the cookie and decide which authentication scheme to use
        options.ForwardDefaultSelector = context =>
        {
            // Check if the default auth cookie exists. If it does, use the cookie authentication scheme.
            if (context.Request.Cookies.ContainsKey(".AspNetCore.Cookies"))
                return CookieAuthenticationDefaults.AuthenticationScheme;

            // Check if the Entra External ID cookie exists
            if (context.Request.Cookies.ContainsKey(".AspNetCore.Cookies.External"))
                return OpenIdConnectDefaults.AuthenticationScheme;

            // Otherwise, fall back to the Identity scheme
            return IdentityConstants.ApplicationScheme;
        };
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        // Cookie settings
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;

        // Expiration settings
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
        
        options.Events.OnRedirectToReturnUrl = context =>
        {
        if (string.IsNullOrEmpty(context.Request.Query["ReturnUrl"]))
        {
            context.Response.Redirect("/Home");
            return Task.CompletedTask;
        }

        context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
        };

        
    });

// Configure authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole(UserRoles.Administrator));

    options.AddPolicy("AdminOrCustomer", policy =>
        policy.RequireRole(UserRoles.Administrator, UserRoles.Customer));
    
    options.AddPolicy("RequireAuth", policy =>
        policy.RequireAuthenticatedUser());
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
        // Enable Swagger UI in development
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "MerchStore API V1");
    });
}
else
{
    app.UseDeveloperExceptionPage(); // Show detailed error messages in development
    // In development, seed the database with test data using the extension method
    app.Services.SeedDatabaseAsync(app.Configuration).Wait();

    // Enable Swagger UI in development
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "MerchStore API V1");
    });
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors("AllowAllOrigins");
// Add authentication middleware
app.UseAuthentication();
// Add authorization middleware
app.UseCookiePolicy();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Landing}/{action=Index}/{id?}")
    .WithStaticAssets();


//app.MapMinimalProductEndpoints();

// Add Razor Pages for Identity UI
app.MapRazorPages();

app.Run();

