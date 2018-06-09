using Microsoft.AspNetCore.Mvc;

namespace GeekCoding.MainApplication.Controllers
{
    public class InterviewController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}