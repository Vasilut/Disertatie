using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GeekCoding.MainApplication.Models;

namespace GeekCoding.MainApplication.Controllers
{
    public class HomeController : Controller
    {

        public HomeController()
        {
        }

        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
