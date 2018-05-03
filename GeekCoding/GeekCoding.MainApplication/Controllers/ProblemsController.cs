using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeekCoding.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GeekCoding.MainApplication.Controllers
{
    public class ProblemsController : Controller
    {
        private IProblemRepository _problemRepository;

        public ProblemsController(IProblemRepository problemRepository)
        {
            _problemRepository = problemRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var problemList = await _problemRepository.GetAllAsync();
            return View();
        }
    }
}