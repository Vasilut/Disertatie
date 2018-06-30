using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using GeekCoding.Compilation.Api.Model;
using GeekCoding.Data.Models;
using GeekCoding.MainApplication.ViewModels;
using GeekCoding.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace GeekCoding.MainApplication.Controllers
{
    [Authorize]
    public class TestsController : Controller
    {
        private ITestsRepository _testRepository;
        private IConfiguration _configuration;
        private IProblemRepository _problemRepository;
        private string _testApi;

        public TestsController(ITestsRepository testsRepository, IConfiguration configuration,
                              IProblemRepository problemRepository)
        {
            _testRepository = testsRepository;
            _configuration = configuration;
            _problemRepository = problemRepository;
            _testApi = _configuration.GetSection("Api")["TestsApi"];
        }
        // GET: Tests
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Index(Guid id)
        {
            var tests = _testRepository.GetTestsByProblemId(id).ToList();
            var testViewModel = new TestViewModel
            {
                ListModel = tests,
                ProblemId = id,
            };

            return View(testViewModel);
        }

        // GET: Tests/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Tests/Create
        public ActionResult Add(Guid id)
        {
            ViewBag.ProblemId = id;
            return View();
        }

        // POST: Tests/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add([FromForm]Tests test)
        {
            if (ModelState.IsValid)
            {
                var newTest = new Tests
                {
                    TestNumber = test.TestNumber,
                    Scor = test.Scor,
                    FisierIn = test.FisierIn,
                    FisierOk = test.FisierOk,
                    TestId = Guid.NewGuid(),
                    ProblemId = test.ProblemId,
                    TestInput = test.TestInput,
                    TestOutput = test.TestOutput
                };

                //save to server the test.
                var problem = _problemRepository.GetItem(newTest.ProblemId);

                //first we save the .in file the the ok file
                TestsModel testsInputModel = new TestsModel
                {
                    Content = newTest.TestInput,
                    FileName = newTest.FisierIn,
                    ProblemName = problem.ProblemName
                };
                var inputFileGenerated = await GenerateInputFile(testsInputModel);
                if (inputFileGenerated == true)
                {
                    //generate for the .ok file
                    var testOkModel = new TestsModel
                    {
                        Content = newTest.TestOutput,
                        FileName = newTest.FisierOk,
                        ProblemName = problem.ProblemName
                    };
                    var outputFileGenerated = await GenerateInputFile(testOkModel);
                    if (outputFileGenerated == true)
                    {
                        //ok, we can save in the database

                        _testRepository.Create(newTest);
                        _testRepository.Save();
                        return RedirectToAction(nameof(Index), new { id = test.ProblemId });
                    }
                    else
                    {
                        return View();
                    }
                }
                else
                {
                    return View();
                }

            }

            return View();
        }

        private async Task<bool> GenerateInputFile(TestsModel testsModel)
        {
            var client = new HttpClient();
            var serializedInputData = JsonConvert.SerializeObject(testsModel);
            var httpContent = new StringContent(serializedInputData, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(_testApi, httpContent);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = await response.Content.ReadAsStringAsync();
                var resultDeserialized = JsonConvert.DeserializeObject(result);
                if (result.Contains("File generated"))
                {
                    //we generated successfuly the input file. we need to generate the output file
                    return true;
                }
                return false;
            }
            return false;
        }

        // GET: Tests/Edit/5
        public ActionResult Edit(Guid id)
        {
            var test = _testRepository.GetItem(id);
            if (test == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(test);
        }

        // POST: Tests/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([FromForm] Tests test)
        {
            if (ModelState.IsValid)
            {
                //update from server first

                _testRepository.Update(test);
                _testRepository.Save();
                return RedirectToAction(nameof(Index), new { id = test.ProblemId });
            }
            return View();
        }

        // GET: Tests/Delete/5
        public ActionResult Delete(Guid id)
        {
            var test = _testRepository.GetItem(id);
            if (test == null)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(test);
        }

        // POST: Tests/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete([FromForm] TestProblemsIdViewModel test)
        {

            //delete from linux server first

            _testRepository.Delete(test.TestId);
            _testRepository.Save();

            return RedirectToAction(nameof(Index), new { id = test.ProblemId });
        }
    }
}