using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeekCoding.Data.Models;
using GeekCoding.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekCoding.MainApplication.Controllers
{
    [Authorize]
    public class ProblemsController : Controller
    {
        private IProblemRepository _problemRepository;

        public ProblemsController(IProblemRepository problemRepository)
        {
            _problemRepository = problemRepository;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var problemList = await _problemRepository.GetAllAsync();
            var goodList = problemList.Where(prob => prob.Visible == true).Select(prop =>
            {
                if (prop.BadSubmission > 0)
                {
                    prop.AverageAcceptance = ((prop.GoodSubmision * 100) / prop.BadSubmission);
                }
                return prop;
            }).ToList();


            return View(goodList);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Add([FromForm] Problem problem)
        {
            if (ModelState.IsValid)
            {
                problem.ProblemId = Guid.NewGuid();
                _problemRepository.Create(problem);
                _problemRepository.Save();
                return RedirectToAction("Index");
            }
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetProblem(Guid id)
        {
            var problem = await _problemRepository.GetAsync(id);
            return View(problem);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public IActionResult Delete(Guid id)
        {
            _problemRepository.Delete(id);
            _problemRepository.Save();
            return View("ProblemDeleted");
        }
    }
}