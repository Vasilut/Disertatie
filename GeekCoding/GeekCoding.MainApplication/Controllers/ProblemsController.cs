using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using GeekCoding.Common.Helpers;
using GeekCoding.Compilation.Api.Model;
using GeekCoding.Data.Models;
using GeekCoding.MainApplication.ViewModels;
using GeekCoding.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace GeekCoding.MainApplication.Controllers
{
    [Authorize]
    public class ProblemsController : Controller
    {
        private IProblemRepository _problemRepository;
        private ISubmisionRepository _submisionRepository;
        private ISolutionRepository _solutionRepository;
        private List<SelectListItem> _compilers = new List<SelectListItem>();
        public object lockOBj = new object();

        public ProblemsController(IProblemRepository problemRepository, ISubmisionRepository submisionRepository, ISolutionRepository solutionRepository)
        {
            _problemRepository = problemRepository;
            _submisionRepository = submisionRepository;
            _solutionRepository = solutionRepository;
            _compilers = Compilator.Compilers;
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
            string fileContent = await FileHelpers.ProcessFormFile(model.File, ModelState);

            //compile file (linux)
            var url = @"http://localhost:32529/api/compilation";
            var client = new HttpClient();
            var compilationModel = new CompilationModel { Content = fileContent, Language = model.Compilator, ProblemName = model.ProblemName, Username = User.Identity.Name };
            var serializedData = JsonConvert.SerializeObject(compilationModel);
            var httpContent = new StringContent(serializedData, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, httpContent);
            var result = await response.Content.ReadAsStringAsync();
            var content = JsonConvert.DeserializeObject<ResponseModel>(result);

            //call the api to execute... not done yet.. (linux)


            //save the submission
            var submission = new Submision
            {
                SubmisionId = Guid.NewGuid(),
                DataOfSubmision = DateTime.Today,
                Compilator = model.Compilator,
                ProblemId = Guid.Parse(model.ProblemId),
                SourceSize = "2kb",
                StateOfSubmision = "Compiled",
                UserName = User.Identity.Name,
                MessageOfSubmision = content.CompilationResponse,
                Score = 100
            };

            await _submisionRepository.AddAsync(submission);

            ViewData["subbmited"] = true;
            return RedirectToAction("GetProblem", new { id = Guid.Parse(model.ProblemId) });
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