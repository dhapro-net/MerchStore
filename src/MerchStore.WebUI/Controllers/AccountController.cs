using System.Security.Claims;
using MerchStore.WebUI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MerchStore.WebUI.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<IdentityUser> _signInManager;

    public AccountController(SignInManager<IdentityUser> signInManager)
    {
        _signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl ?? Url.Action("Index", "Home");
        return View();
    }

    [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> LoginAsync(LoginViewModel model, string? returnUrl = null)
{
    ViewData["ReturnUrl"] = returnUrl;

    if (!ModelState.IsValid)
    {
        return View(model);
    }
    
    // 🛡️ SAFETY CHECK: Make sure we have actual values, not empty boxes!
    if (string.IsNullOrWhiteSpace(model.Username))
    {
        ModelState.AddModelError(nameof(model.Username), "Username is required.");
        return View(model);
    }
    
    if (string.IsNullOrWhiteSpace(model.Password))
    {
        ModelState.AddModelError(nameof(model.Password), "Password is required.");
        return View(model);
    }
    
    // ✅ NOW we know Username and Password are NOT null!
    var result = await _signInManager.PasswordSignInAsync(
        model.Username, model.Password, isPersistent: true, lockoutOnFailure: false);
        
    if (result.Succeeded)
    {
        // Successful login
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        return RedirectToAction("Index", "Home");
    }

    // Failed login - Don't tell attackers whether username or password was wrong!
    ModelState.AddModelError(string.Empty, "Invalid username or password.");
    return View(model);
}

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}