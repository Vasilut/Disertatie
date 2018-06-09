using Microsoft.AspNetCore.Mvc;

namespace GeekCoding.MainApplication.Controllers
{
    public class ContestController : Controller
    {
        public ContestController()
        {

        }
        public IActionResult Index()
        {
            return View();
        }
    }
}