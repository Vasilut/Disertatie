using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeekCoding.Compilation.Api.Model;
using GeekCoding.Data.Models;
using GeekCoding.MainApplication.Jobs;
using GeekCoding.MainApplication.Utilities;
using GeekCoding.Repository.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace GeekCoding.MainApplication.Controllers
{
    //it will be api
    public class SubmissionController : Controller
    {
        private ISubmisionRepository _submisionRepository;
        private IConfiguration _configuration;
        private IProblemRepository _problemRepository;
        private string _compilationApi;
        private string _executionApi;

        public SubmissionController(ISubmisionRepository submisionRepository, IConfiguration configuration, IProblemRepository problemRepository)
        {
            _submisionRepository = submisionRepository;
            _configuration = configuration;
            _problemRepository = problemRepository;

            //intialize compilation and running api
            _compilationApi = _configuration.GetSection("Api")["CompilationApi"];
            _executionApi = _configuration.GetSection("Api")["ExecutionApi"];
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            
            var submisionList = await _submisionRepository.GetAllAsync();
            //need to get all the submission that have queed flag set to 0
            foreach (var submission in submisionList)
            {
                if(submission.JobQueued == false)
                {
                    //start a job for this submission
                    var problem = _problemRepository.GetItem(submission.ProblemId);
                    string problemName = problem.ProblemName;

                    var submissionDtoModel = new SubmisionDto { Compilator = submission.Compilator, ProblemName = problemName, Content = submission.SourceCode, SubmissionId = submission.SubmisionId, UserName = User.Identity.Name };
                    BackgroundJob.Enqueue<SubmissionRequest>(x => x.MakeSubmissionRequestAsync(submissionDtoModel, _compilationApi,_executionApi));
                    submission.JobQueued = true;
                }
                
            }
            _submisionRepository.Save();
            return View(submisionList);
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
            return View(submision);
        }
    }
}