
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace MerchStore.WebUI.Controllers;


public class AuthController : Controller
{
    [HttpGet]
    public IActionResult Login()
    {
        return View(); // will show Views/Auth/Login.cshtml
    }

    [HttpGet]
    public IActionResult LoginWithMicrosoft(string returnUrl = "/")
    {
        return Challenge(
        new AuthenticationProperties
        {
            RedirectUri = returnUrl
        },
        OpenIdConnectDefaults.AuthenticationScheme 
        );
    }
}