using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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