using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeekCoding.Compilation.Api.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GeekCoding.Compilation.Api.Controllers
{
    [Route("api/[controller]")]
    public class CompilationController : Controller
    {
        private ICompilationFile _compilationFile;

        public CompilationController(ICompilationFile compilationFile)
        {
            _compilationFile = compilationFile;
        }
        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "compile1", "compile2" };
        }

        
        // POST api/<controller>
        [HttpPost]
        public string Post([FromBody]CompilationModel item)
        {
            if (ModelState.IsValid)
            {
                var response = _compilationFile.CompileFile(item.Content, item.Language, item.ProblemName, item.Username);
                return JsonConvert.SerializeObject(new ResponseModel {
                                                   CompilationResponse = response.Item1.ToString(),
                                                   OutputMessage = response.Item2 });
            }

            return "Failed response";
        }

        
    }
}
