using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeekCoding.Compilation.Api.Model;
using GeekCoding.Compilation.GenerateTests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GeekCoding.Compilation.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/Tests")]
    public class TestsController : Controller
    {
        private ITestGenerator _testGenerator;

        public TestsController(ITestGenerator testGenerator)
        {
            _testGenerator = testGenerator;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "testapigoodye2bunaziuaaavv", "testapibadye2bunaziuaavv" };
        }

        [HttpPost]
        public JsonResult Post([FromBody] TestsModel item)
        {
            if(ModelState.IsValid)
            {
                _testGenerator.GenerateFile(item.FileName, item.Content, item.ProblemName);
                return Json("File generated");
            }
            return Json("Failed Response");
        }

        [HttpPost("deleteTest")]
        public JsonResult DeleteTestFiles([FromBody] TestDeleteModel item)
        {
            if(ModelState.IsValid)
            {
                var deleted = _testGenerator.DeleteTestFile(item.FisierIn, item.FisierOk, item.ProblemName);
                if (deleted == true)
                {
                    return Json("File deleted");
                }
                return Json("Failed to delete files");
            }
            return Json("Failed response");
        }
    }
}