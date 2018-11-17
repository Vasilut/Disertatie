using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GeekCoding.Compilation.Api.Model;
using GeekCoding.Data.Models;
using GeekCoding.MainApplication.Hubs;
using GeekCoding.MainApplication.Jobs;
using GeekCoding.MainApplication.Pagination;
using GeekCoding.MainApplication.Utilities;
using GeekCoding.MainApplication.Utilities.DTO;
using GeekCoding.MainApplication.ViewModels;
using GeekCoding.Repository.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace GeekCoding.MainApplication.Controllers
{
    //it will be api
    public class SubmissionController : Controller
    {
        private ISubmisionRepository _submisionRepository;
        private IConfiguration _configuration;
        private IProblemRepository _problemRepository;
        private IEvaluationRepository _evaluationRepository;
        private ITestsRepository _testRepository;
        private string _compilationApi;
        private string _executionApi;

        public SubmissionController(ISubmisionRepository submisionRepository, IConfiguration configuration,
                                    IProblemRepository problemRepository, IEvaluationRepository evaluationRepository,
                                    ITestsRepository testsRepository)
        {
            _submisionRepository = submisionRepository;
            _configuration = configuration;
            _problemRepository = problemRepository;
            _evaluationRepository = evaluationRepository;
            _testRepository = testsRepository;

            //intialize compilation and running api
            _compilationApi = _configuration.GetSection("Api")["CompilationApi"];
            _executionApi = _configuration.GetSection("Api")["ExecutionApi"];
        }


        [HttpGet]
        public async Task<IActionResult> Index(int? page)
        {

            /*
             *
             static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
                    semaphoreSlim.Wait();
                    code to execute
                    semaphoreSlim.Release();

             */

            var submisionList = await _submisionRepository.GetAllAsync();
            var submissionListOrderedAsc = submisionList.OrderBy(x => x.DataOfSubmision).ToList();
            //need to get all the submission that have queed flag set to 0
            foreach (var submission in submissionListOrderedAsc)
            {

                if (submission.JobQueued == false)
                {
                    //start a job for this submission
                    var problem = _problemRepository.GetItem(submission.ProblemId);
                    string problemName = problem.ProblemName;
                    var tests = _testRepository.GetTestsByProblemId(problem.ProblemId).ToList();
                    int nrOfTests = tests.Count;
                    string nameOfFile = problemName.ToLower();

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
                        FileName = nameOfFile
                    };
                    BackgroundJob.Enqueue<SubmissionRequest>(x => x.MakeSubmissionRequestAsync(submissionDtoModel, _compilationApi, _executionApi));
                    submission.JobQueued = true;
                }

            }
            _submisionRepository.Save();
            int pageSize = 20;
            var submissionListOrdered = submisionList.OrderByDescending(sub => sub.DataOfSubmision).ToList();
            return View(PaginatedList<Submision>.CreateAsync(submissionListOrdered, page ?? 1, pageSize));
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost] //it will be api
        public async Task CreateSubmision([FromBody] Submision submision)
        {
            await _submisionRepository.AddAsync(submision);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var submision = await _submisionRepository.GetAsync(id);
            var problem = submision.Problem;
            var evaluationResult = _evaluationRepository.GetItemBySubmission(id);
            var lstEvaluationResult = EvaluationList(evaluationResult);

            SubmisionDetailsViewModel sd = new SubmisionDetailsViewModel
            {
                UserName = submision.UserName,
                Data = submision.DataOfSubmision,
                ProblemName = problem.ProblemName,
                Status = submision.StateOfSubmision,
                Compilator = submision.Compilator,
                SourceSize = submision.SourceSize,
                ProblemTimeLimit = problem.TimeLimit,
                ProblemMemoryLimit = problem.MemoryLimit,
                Scor = submision.Score,
                EvaluationResult = lstEvaluationResult,
                MessageOfSubmission = submision.MessageOfSubmision

            };

            return View(sd);
        }

        private List<TestModelDto> EvaluationList(Evaluation evaluation)
        {
            if (evaluation == null)
            {
                return new List<TestModelDto>();
            }
            var list = JsonConvert.DeserializeObject<List<TestModelDto>>(evaluation.EvaluationResult);
            return list;
        }
    }
}