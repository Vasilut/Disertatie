using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeekCoding.Data.Models;
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
            return View(problemList);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add([FromBody] Problem problem)
        {
            _problemRepository.Create(problem);
            _problemRepository.Save();
            return View("ProblemAdded");
        }

        [HttpGet]
        public async Task<IActionResult> GetProblem(Guid id)
        {
            var problem = await _problemRepository.GetAsync(id);
            return View(problem);
        }

        [HttpDelete]
        public IActionResult Delete(Guid id)
        {
            _problemRepository.Delete(id);
            _problemRepository.Save();
            return View("ProblemDeleted");
        }
    }
}