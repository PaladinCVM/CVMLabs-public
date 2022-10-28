using Microsoft.AspNetCore.Mvc;

namespace CVMLabs.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();

        public IActionResult About() => View();
    }
}