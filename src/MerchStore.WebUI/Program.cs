using MerchStore.Application.Services.Implementations;
using MerchStore.Application.Services.Interfaces;
using MerchStore.Infrastructure.Persistence;          
using MerchStore.WebUI.Authentication.ApiKey;
using MerchStore.WebUI.Endpoints;                     // for SecurityRequirementsOperationFilter
using MerchStore.WebUI.Infrastructure;                
using MerchStore.WebUI.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using MerchStore.Application;                         
using MerchStore.Infrastructure;
using System.Text.Json.Serialization; 
using Azure.Identity;
using dotenv.net;

var builder = WebApplication.CreateBuilder(args);

// Add Key Vault to configuration
/*var keyVaultName = builder.Configuration["Config:AzureKeyVaultName"]; 
if (!string.IsNullOrEmpty(keyVaultName))
{
    var keyVaultUri = new Uri($"https://{keyVaultName}.vault.azure.net/");
    builder.Configuration.AddAzureKeyVault(keyVaultUri, new DefaultAzureCredential());

    // 🧪 Test the CosmosDB secret was loaded
    var cosmosTest = builder.Configuration["CosmosDb:ConnectionString"];
    Console.WriteLine("🔑 CosmosDB connection from Key Vault: " + cosmosTest);
}*/
// Update the JSON options configuration to use our custom policy
DotEnv.Load(); // from dotenv.net
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddControllersWithViews()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.PropertyNamingPolicy   = new JsonSnakeCaseNamingPolicy();
        opts.JsonSerializerOptions.DictionaryKeyPolicy    = new JsonSnakeCaseNamingPolicy();
        opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Razor-Pages needed for Identity UI
builder.Services.AddRazorPages();

// Prevent JWT handler from remapping claims
JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

// Register DbContext with Identity
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("MerchStoreDb"));

// ──────────── Identity with UI & EF Core ────────────
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    {
        // your password / lockout / sign-in options
        options.SignIn.RequireConfirmedAccount = false;
        options.User.RequireUniqueEmail = true;
    })
    .AddRoles<IdentityRole>()                   //roles
    .AddEntityFrameworkStores<AppDbContext>()   // run User Admin on inmemory-DB
    .AddDefaultTokenProviders()
    .AddDefaultUI();                            // wires up /Areas/Identity Razor-Pages

// ──────────── API Key Authentication ────────────
builder.Services.AddAuthentication(options =>
    {
        // leave DefaultScheme alone so Identity cookies still work
    })
    .AddApiKey(builder.Configuration["ApiKey:Value"]
        ?? throw new InvalidOperationException("API Key not configured"));

// ──────────── Authorization ────────────
builder.Services.AddAuthorization(opts =>
{
    opts.AddPolicy("ApiKeyPolicy", policy =>
        policy.AddAuthenticationSchemes(ApiKeyAuthenticationDefaults.AuthenticationScheme)
              .RequireAuthenticatedUser());

    opts.AddPolicy("RequireAuth", policy =>
        policy.RequireAuthenticatedUser());

    opts.AddPolicy("AdminOnly", policy =>
        policy.RequireRole(UserRoles.Administrator));

    opts.AddPolicy("AdminOrCustomer", policy =>
        policy.RequireRole(UserRoles.Administrator, UserRoles.Customer));
});

// ──────────── Application & Infrastructure ────────────
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
Console.WriteLine("🌍 Config:DatabaseType = " + builder.Configuration["Config:DatabaseType"]);
//builder.Services.AddScoped<ICatalogService, CatalogService>();

// ──────────── Shopping Cart & HTTP Context ────────────
builder.Services.AddScoped<CookieShoppingCartService>();
builder.Services.AddHttpContextAccessor();

// ──────────── Swagger / OpenAPI ────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opts =>
{
    opts.SwaggerDoc("v1", new OpenApiInfo
    {
        Title       = "MerchStore API",
        Version     = "v1",
        Description = "API for MerchStore product catalog",
        Contact     = new OpenApiContact { Name = "MerchStore Support", Email = "support@merchstore.example.com" }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        opts.IncludeXmlComments(xmlPath);

    opts.CustomOperationIds(desc =>
        desc.TryGetMethodInfo(out var mi) ? mi.Name : null);

    // API Key in Swagger UI
    opts.AddSecurityDefinition(ApiKeyAuthenticationDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Description = "API Key Authentication. Enter your API key in the header.",
        Name        = ApiKeyAuthenticationDefaults.HeaderName,
        In          = ParameterLocation.Header,
        Type        = SecuritySchemeType.ApiKey,
        Scheme      = ApiKeyAuthenticationDefaults.AuthenticationScheme
    });
    opts.OperationFilter<SecurityRequirementsOperationFilter>();
});

// ──────────── CORS & Logging ────────────
builder.Services.AddCors(o => o.AddPolicy("AllowAll", p => p
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
));
builder.Services.AddLogging(b => {
    b.AddConsole();
    b.AddDebug();
});

var app = builder.Build();

// ──────────── Auto-Create Admin User ────────────
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    
    // Create Administrator role if it doesn't exist
    if (!await roleManager.RoleExistsAsync(UserRoles.Administrator))
    {
        await roleManager.CreateAsync(new IdentityRole(UserRoles.Administrator));
    }
    
    // Create Customer role too
    if (!await roleManager.RoleExistsAsync(UserRoles.Customer))
    {
        await roleManager.CreateAsync(new IdentityRole(UserRoles.Customer));
    }
    
    // Create default admin user
    var adminEmail = "admin@merchstore.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    
    if (adminUser == null)
    {
        adminUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };
        
        await userManager.CreateAsync(adminUser, "Admin123!");
        await userManager.AddToRoleAsync(adminUser, UserRoles.Administrator);
        
        app.Logger.LogInformation("🔐 Created default admin user: admin@merchstore.com");
    }
}



// ──────────── Middleware Pipeline ────────────
if (app.Environment.IsDevelopment())
{
    // Use custom error page in production
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();

    // Enable Swagger UI in development
    app.Services.SeedDatabaseAsync(app.Configuration).Wait();

    // Enable Swagger UI in development
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "MerchStore API V1");
    });

}
else
{
    // Show detailed error messages in development
    app.UseDeveloperExceptionPage();
    // Seed database with test data in development
    app.Services.SeedDatabaseAsync(app.Configuration).Wait();
    
}

// ✅ Enable Swagger in all environments
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "MerchStore API V1");
});


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

// ──────────── Routing ────────────
//  1) Razor-Pages first so that /Identity/Account/Login is handled by the Identity UI
app.MapRazorPages();

//  2) MVC controllers
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Landing}/{action=Index}/{id?}"
);

//  3) (Optional) Map any minimal-API endpoints in WebUI.Endpoints
// app.MapMinimalProductEndpoints();

app.Run();
