using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeekCoding.Data.Models;
using GeekCoding.MainApplication.ViewModels;
using GeekCoding.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace GeekCoding.MainApplication.Controllers
{
    [Authorize]
    public class TestsController : Controller
    {
        private ITestsRepository _testRepository;
        private IConfiguration _configuration;
        private string _testApi;

        public TestsController(ITestsRepository testsRepository, IConfiguration configuration)
        {
            _testRepository = testsRepository;
            _configuration = configuration;
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
        public ActionResult Add([FromForm]Tests test)
        {
            if(ModelState.IsValid)
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

                _testRepository.Create(newTest);
                _testRepository.Save();
                
                return RedirectToAction(nameof(Index), new { id = test.ProblemId });
            }

            return View();
        }

        // GET: Tests/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Tests/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Tests/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Tests/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}