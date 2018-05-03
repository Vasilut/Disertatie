using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GeekCoding.MainApplication.Models;
using GeekCoding.Repository.Interfaces;

namespace GeekCoding.MainApplication.Controllers
{
    public class HomeController : Controller
    {
        private IProblemRepository _repository;

        public HomeController(IProblemRepository repository )
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            var lst = _repository.GetAll().ToList();
            return View();
        }
        
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
