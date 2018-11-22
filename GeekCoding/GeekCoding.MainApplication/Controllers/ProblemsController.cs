using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GeekCoding.Common.Helpers;
using GeekCoding.Compilation.Api.Model;
using GeekCoding.Data.Models;
using GeekCoding.MainApplication.Hubs;
using GeekCoding.MainApplication.Jobs;
using GeekCoding.MainApplication.Pagination;
using GeekCoding.MainApplication.Utilities;
using GeekCoding.MainApplication.ViewModels;
using GeekCoding.Repository.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace GeekCoding.MainApplication.Controllers
{
    [Authorize]
    public class ProblemsController : Controller
    {
        private IProblemRepository _problemRepository;
        private SubmissionHub _submissionHub;
        private ISubmisionRepository _submisionRepository;
        private ISolutionRepository _solutionRepository;
        private IConfiguration _configuration;
        private ITestsRepository _testRepository;
        private IEvaluationRepository _evaluationRepository;
        private IHubContext<SubmissionHub> _hubContext;
        private ISerializeTests _serializeTests;
        private List<SelectListItem> _compilers = new List<SelectListItem>();
        static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        private string _compilationApi;
        private string _executionApi;

        public ProblemsController(IProblemRepository problemRepository, ISubmisionRepository submisionRepository,
                                  ISolutionRepository solutionRepository, IConfiguration configuration,
                                  ITestsRepository testsRepository, SubmissionHub submissionHub,
                                 IHubContext<SubmissionHub> hubContext, ISerializeTests serializeTests,
                                 IEvaluationRepository evaluationRepository)
        {
            _problemRepository = problemRepository;
            _submisionRepository = submisionRepository;
            _solutionRepository = solutionRepository;
            _configuration = configuration;
            _testRepository = testsRepository;
            _submissionHub = submissionHub;
            _evaluationRepository = evaluationRepository;
            _hubContext = hubContext;
            _serializeTests = serializeTests;
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
            var goodList = new List<Problem>();

            if (User.Identity.Name == null || !User.IsInRole("Admin"))
            {
                //we don't have an admin, show only the visible problem
                goodList = problemList.Where(prob => prob.Visible == true).ToList();
            }
            else
            {
                goodList = problemList.ToList();
            }

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
            if (problem == null)
            {
                return RedirectToAction("Index");
            }
            return View(problem);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Edit([FromForm] Problem problem)
        {
            if (ModelState.IsValid)
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
            var solution = _solutionRepository.GetSolutionByProblem(id);

            var submisionList = _submisionRepository.GetSubmisionByProblemIdAndUserName(id, User.Identity.Name)
                                                    .OrderByDescending(sub => sub.DataOfSubmision).ToList();
            
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
            var solution = _solutionRepository.GetSolutionByProblem(id);
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
            
            //build the submission dto
            var problem = _problemRepository.GetItem(submission.ProblemId);
            string problemName = problem.ProblemName;
            int nrOfTests = _testRepository.GetNumberOfTestForProblem(problem.ProblemId);

            var submissionDtoModel = new SubmisionDto
            {
                Compilator = submission.Compilator,
                ProblemName = problemName,
                Content = submission.SourceCode,
                SubmissionId = submission.SubmisionId,
                UserName = User.Identity.Name,
                MemoryLimit = problem.MemoryLimit,
                TimeLimit = problem.TimeLimit,
                NumberOfTests = nrOfTests,
                FileName = problemName.ToLower()
            };

            await _submisionRepository.AddAsync(submission);
            await Task.Run(() => VerificaThread(submissionDtoModel));
            
            return RedirectToAction("GetProblem", new { id = Guid.Parse(model.ProblemId) });
        }


        private async Task VerificaThread(SubmisionDto submissionDtoModel)
        {
            semaphoreSlim.Wait();
            try
            {
                var submRequest = new SubmissionRequest(_submissionHub, _submisionRepository, _hubContext,
                                                        _serializeTests, _evaluationRepository);
                await submRequest.MakeSubmissionRequestAsync(submissionDtoModel, _compilationApi, _executionApi);
            }
            finally
            {
                semaphoreSlim.Release();
            }
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

        [Authorize(Roles = ("Admin"))]
        [HttpPost]
        public IActionResult DeleteConfirmed(Guid problemId)
        {
            _problemRepository.Delete(problemId);
            _problemRepository.Save();
            return View("Index");
        }
    }
}