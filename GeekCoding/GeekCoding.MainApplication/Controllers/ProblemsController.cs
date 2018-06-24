using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using GeekCoding.Common.Helpers;
using GeekCoding.Compilation.Api.Model;
using GeekCoding.Data.Models;
using GeekCoding.MainApplication.Jobs;
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
                    Visible = problem.Visible

                };

                //create problem directory via api (linux hosted)

                _problemRepository.Create(problemNew);
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
            var tp = new Tuple<Problem, List<SelectListItem>>(problem, _compilers);
            return View(tp);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Submissions(Guid id)
        {
            var listOfSubmission = await _submisionRepository.GetAllAsync();
            var submisionList = listOfSubmission.Where(sub => sub.ProblemId == id).ToList();

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
            Tuple<string,long> fileContent = await FileHelpers.ProcessFormFile(model.File, ModelState);
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

            //compile file (linux)
            
            /*BackgroundJob.Enqueue<SubmissionRequest>(x => x.MakeSubmissionRequestAsync(compilationModel, _compilationApi,
                                                                                       User.Identity.Name, _executionApi,
                                                                                       submission.SubmisionId.ToString()));
                                                                                       */
            //var client = new HttpClient();
            //var serializedData = JsonConvert.SerializeObject(compilationModel);
            //var httpContent = new StringContent(serializedData, Encoding.UTF8, "application/json");

            //var response = await client.PostAsync(_compilationApi, httpContent);
            //if (response.StatusCode == System.Net.HttpStatusCode.OK)
            //{
            //    var result = await response.Content.ReadAsStringAsync();
            //    var content = JsonConvert.DeserializeObject<ResponseModel>(result);

                

                //if (content.CompilationResponse == "SUCCESS")
                //{
                //    //call the api to execute... not done yet.. (linux)
                //    var executionModel = new ExecutionModel { MemoryLimit = "10000", ProblemName = model.ProblemName, UserName = User.Identity.Name, TimeLimit = "2" };
                //    var serializedExecutionData = JsonConvert.SerializeObject(executionModel);
                //    var httpContentExecution = new StringContent(serializedExecutionData, Encoding.UTF8, "application/json");
                //    var responseExecution = await client.PostAsync(_executionApi, httpContent);
                //    if(responseExecution.StatusCode == System.Net.HttpStatusCode.OK)
                //    {
                //        var resultEx = await responseExecution.Content.ReadAsStringAsync();
                //        var x = 2;
                //    }

                //}
                

                ViewData["subbmited"] = true;
                return RedirectToAction("GetProblem", new { id = Guid.Parse(model.ProblemId) });
            }

        //    return RedirectToAction("GetProblem", new { id = Guid.Parse(model.ProblemId) });
        //}


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