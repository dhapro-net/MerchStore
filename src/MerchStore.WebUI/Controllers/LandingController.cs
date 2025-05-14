
using Microsoft.AspNetCore.Mvc;

namespace MerchStore.WebUI.Controllers;


public class LandingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
