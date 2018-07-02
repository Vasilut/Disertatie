using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeekCoding.Data.Models;
using GeekCoding.MainApplication.ViewModels;
using GeekCoding.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace GeekCoding.MainApplication.Controllers
{
    [Authorize]
    public class ArticlesController : Controller
    {
        private IProblemRepository _problemRepository;
        private ISolutionRepository _solutionRepository;

        public ArticlesController(IProblemRepository problemRepository, ISolutionRepository solutionRepository)
        {
            _problemRepository = problemRepository;
            _solutionRepository = solutionRepository;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Index()
        {
            var lst = _solutionRepository.GetAll().ToList();
            return View(lst);
        }
        
        [Authorize(Roles ="Admin")]
        [HttpGet]
        public IActionResult ProblemSolution(Guid id)
        {
            var solution = _solutionRepository.GetSolutionByProblem(id);
            var problem = _problemRepository.GetItem(id);

            var solModel = new SolutionViewModel
            {
                ProblemId = id,
                ProblemName = problem.ProblemName,
                Solution = solution
            };
            return View(solModel);
        }

        public IActionResult Add(Guid id)
        {
            ViewBag.ProblemId = id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add([FromForm] Solution solution)
        {
            if (ModelState.IsValid)
            {
                var solutionToBeInserted = new Solution
                {
                    SolutionId = Guid.NewGuid(),
                    ProblemId = solution.ProblemId,
                    Author = solution.Author,
                    DateAdded = DateTime.Now,
                    Content = solution.Content,
                    Name = solution.Name,
                    Visible = solution.Visible
                };
                _solutionRepository.Create(solutionToBeInserted);
                _solutionRepository.Save();
                return RedirectToAction(nameof(ProblemSolution), new { id = solution.ProblemId });
            }
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Edit(Guid id)
        {
            var solution = _solutionRepository.GetItem(id);
            if(solution == null)
            {
                return RedirectToAction(nameof(ProblemSolution));
            }
            return View(solution);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Edit([FromForm] Solution solution)
        {
            if(ModelState.IsValid)
            {
                //update entity
                _solutionRepository.Update(solution);
                _solutionRepository.Save();
                return RedirectToAction(nameof(ProblemSolution), new { id = solution.ProblemId });
            }
            return View();
        }

        [Authorize(Roles ="Admin")]
        [HttpGet]
        public IActionResult Delete(Guid id)
        {
            var solution = _solutionRepository.GetItem(id);
            if(solution == null)
            {
                return RedirectToAction(nameof(ProblemSolution));
            }
            return View(solution);
        }

        [Authorize(Roles ="Admin")]
        [HttpPost]
        public IActionResult Delete([FromForm] SolutionProblemViewModel solution)
        {
            _solutionRepository.Delete(solution.SolutionId);
            _solutionRepository.Save();

            return RedirectToAction(nameof(ProblemSolution), new { id = solution.ProblemId });
        }
    }
}