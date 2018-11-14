﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using GeekCoding.Common.Helpers;
using GeekCoding.Compilation.Api.Model;
using GeekCoding.Data.Models;
using GeekCoding.MainApplication.Jobs;
using GeekCoding.MainApplication.Pagination;
using GeekCoding.MainApplication.Utilities;
using GeekCoding.MainApplication.ViewModels;
using GeekCoding.Repository.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace GeekCoding.MainApplication.Controllers
{
    [Authorize]
    public class ProblemsController : Controller
    {
        private IProblemRepository _problemRepository;
        private ISubmisionRepository _submisionRepository;
        private ISolutionRepository _solutionRepository;
        private IConfiguration _configuration;
        private List<SelectListItem> _compilers = new List<SelectListItem>();
        public object lockOBj = new object();

        private string _compilationApi;
        private string _executionApi;

        public ProblemsController(IProblemRepository problemRepository, ISubmisionRepository submisionRepository,
                                  ISolutionRepository solutionRepository, IConfiguration configuration)
        {
            _problemRepository = problemRepository;
            _submisionRepository = submisionRepository;
            _solutionRepository = solutionRepository;
            _configuration = configuration;
            _compilers = Compilator.Compilers;

            //intialize compilation and running api
            _compilationApi = _configuration.GetSection("Api")["CompilationApi"];
            _executionApi = _configuration.GetSection("Api")["ExecutionApi"];
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index(int? page)
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

            int pageSize = 20;
            return View(PaginatedList<Problem>.CreateAsync(goodList, page ?? 1, pageSize));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Add([FromForm] ProblemViewModel problem)
        {
            if (ModelState.IsValid)
            {
                var problemNew = new Problem
                {
                    ProblemId = Guid.NewGuid(),
                    ProblemName = problem.ProblemName,
                    ProblemContent = problem.ProblemContent,
                    Dificulty = problem.Dificulty,
                    Visible = problem.Visible,
                    MemoryLimit = problem.MemoryLimit,
                    TimeLimit = problem.TimeLimit

                };

                //create problem directory via api (linux hosted)

                _problemRepository.Create(problemNew);
                _problemRepository.Save();
                return RedirectToAction("Index");
            }
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Edit(Guid id)
        {
            var problem = _problemRepository.GetItem(id);
            if(problem == null)
            {
                return RedirectToAction("Index");
            }
            return View(problem);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Edit([FromForm] Problem problem)
        {
            if(ModelState.IsValid)
            {
                //update the entity;
                _problemRepository.Update(problem);
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
            var listOfSubmission = await _submisionRepository.GetAllAsync();
            var submisionList = listOfSubmission.Where(sub => sub.ProblemId == id && sub.UserName == User.Identity.Name)
                                                .OrderByDescending(sub => sub.DataOfSubmision).ToList();

            var solutions = await _solutionRepository.GetAllAsync();
            var solution = solutions.Where(sol => sol.ProblemId == id).FirstOrDefault();
            var score = submisionList.FirstOrDefault() == null ? 0 : submisionList.First().Score;

            ProblemDetailsViewModel problemDetailsViewModel = new ProblemDetailsViewModel
            {
                Problem = problem,
                Submisions = submisionList,
                Solution = solution,
                SelectListItems = _compilers,
                Score = score
            };
            return View(problemDetailsViewModel);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Submissions(Guid id)
        {
            var listOfSubmission = await _submisionRepository.GetAllAsync();
            var submisionList = listOfSubmission.Where(sub => sub.ProblemId == id && sub.UserName == User.Identity.Name)
                                                .OrderByDescending(sub => sub.DataOfSubmision).ToList();

            return View(submisionList);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Solution(Guid id)
        {
            var solutions = await _solutionRepository.GetAllAsync();
            var solution = solutions.Where(sol => sol.ProblemId == id).FirstOrDefault();

            return View(solution);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ProblemExecute([FromForm] FileExecutionViewModel model)
        {
            //read the content of the file
            Tuple<string, long> fileContent = await FileHelpers.ProcessFormFile(model.File, ModelState);
            double sizeOfFile = (fileContent.Item2) % 1000;
            var compilationModel = new CompilationModel { Content = fileContent.Item1, Language = model.Compilator, ProblemName = model.ProblemName, Username = User.Identity.Name };

            //save the submission
            var submission = new Submision
            {
                SubmisionId = Guid.NewGuid(),
                DataOfSubmision = DateTime.Now,
                Compilator = model.Compilator,
                ProblemId = Guid.Parse(model.ProblemId),
                SourceSize = sizeOfFile.ToString(),
                StateOfSubmision = SubmissionStatus.NotCompiled.ToString(),
                UserName = User.Identity.Name,
                MessageOfSubmision = string.Empty,
                Score = 0,
                JobQueued = false,
                SourceCode = fileContent.Item1
            };

            await _submisionRepository.AddAsync(submission);

            ViewData["subbmited"] = true;
            return RedirectToAction("GetProblem", new { id = Guid.Parse(model.ProblemId) });
        }
        

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Delete(Guid id)
        {
            var problem = _problemRepository.GetItem(id);
            if (problem == null)
            {
                return RedirectToAction("Index");
            }
            return View(problem);
        }

        [Authorize(Roles =("Admin"))]
        [HttpPost]
        public IActionResult DeleteConfirmed(Guid problemId)
        {
            _problemRepository.Delete(problemId);
            _problemRepository.Save();
            return View("Index");
        }
    }
}