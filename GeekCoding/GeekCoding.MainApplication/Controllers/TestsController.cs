using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using GeekCoding.Common.Helpers;
using GeekCoding.Compilation.Api.Model;
using GeekCoding.Data.Models;
using GeekCoding.MainApplication.Utilities.Enum;
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
        private string _deleteTestApi;

        public TestsController(ITestsRepository testsRepository, IConfiguration configuration,
                              IProblemRepository problemRepository)
        {
            _testRepository = testsRepository;
            _configuration = configuration;
            _problemRepository = problemRepository;
            _testApi = _configuration.GetSection("Api")["TestsApi"];
            _deleteTestApi = _testApi + "/deleteTest";
        }
        // GET: Tests
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Index(Guid id)
        {
            var someProperty = _testRepository.GetSomePropertyForTests(id);

           // var tests = _testRepository.GetTestsByProblemId(id).ToList();
            var testViewModel = new TestViewModel
            {
                ListModel = someProperty.ToList(),
                ProblemId = id,
            };

            return View(testViewModel);
        }

        // GET: Tests/Details/5
        public IActionResult Details(int id)
        {
            return View();
        }

        // GET: Tests/Create
        public IActionResult Add(Guid id)
        {
            ViewBag.ProblemId = id;
            return View();
        }

        [HttpGet]
        public IActionResult TestDownload(Guid id, string Download)
        {
            var test = _testRepository.GetItem(id);
            if(test != null)
            {
                string testToDownload = string.Empty;
                if(Download == TestType.Input.ToString())
                {
                    testToDownload = test.TestInput;
                }
                else
                if(Download == TestType.Output.ToString())
                {
                    testToDownload = test.TestOutput;
                }

                return Ok(testToDownload);

            }
            return Ok("Test is null!");
        }

        // POST: Tests/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add([FromForm]TestsViewModel test)
        {
            if (ModelState.IsValid)
            {
                string testInputContent = string.Empty;
                string testOutputContent = string.Empty;

                Tuple<string, long> fileContentInput = await FileHelpers.ProcessFormFile(test.FileInput, ModelState);
                Tuple<string,long> fileContentOutput = await FileHelpers.ProcessFormFile(test.FileOutput, ModelState);

                testInputContent = fileContentInput.Item1;
                testOutputContent = fileContentOutput.Item1;

                var newTest = new Tests
                {
                    TestNumber = test.TestNumber,
                    Scor = test.Scor,
                    FisierIn = test.FisierIn,
                    FisierOk = test.FisierOk,
                    TestId = Guid.NewGuid(),
                    ProblemId = test.ProblemId,
                    TestInput = testInputContent,
                    TestOutput = testOutputContent
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
        public IActionResult Edit(Guid id)
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
        public IActionResult Edit([FromForm] Tests test)
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
        public IActionResult Delete(Guid id)
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
        public async Task<IActionResult> Delete([FromForm] TestProblemsIdViewModel test)
        {
            var testToDelete = _testRepository.GetItem(test.TestId);
            if(testToDelete == null)
            {
                return BadRequest("No test to delete");
            }

            var problem = _problemRepository.GetItem(test.ProblemId);
            var problemName = problem?.ProblemName;

            //delete from linux server first
            TestDeleteModel testToDeleteModel = new TestDeleteModel
            {
                ProblemName = problemName,
                FisierIn = testToDelete.FisierIn,
                FisierOk = testToDelete.FisierOk
            };

            var testWasDeleted = await DeleteTest(testToDeleteModel);
            if (testWasDeleted == true)
            {
                _testRepository.Delete(test.TestId);
                _testRepository.Save();

                return RedirectToAction(nameof(Index), new { id = test.ProblemId });
            }
            return RedirectToAction(nameof(Delete), new { id = test.TestId });
        }

        private async Task<bool> DeleteTest(TestDeleteModel testToDeleteModel)
        {
            var client = new HttpClient();
            var serializedInputData = JsonConvert.SerializeObject(testToDeleteModel);
            var httpContent = new StringContent(serializedInputData, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(_deleteTestApi, httpContent);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = await response.Content.ReadAsStringAsync();
                var resultDeserialized = JsonConvert.DeserializeObject(result);
                if (result.Contains("File deleted"))
                {
                    //we delete successfuly the input and the output file
                    return true;
                }
                return false;
            }
            return false;
        }
    }
}