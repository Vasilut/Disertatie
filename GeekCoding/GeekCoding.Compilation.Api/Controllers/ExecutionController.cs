using System.Collections.Generic;
using GeekCoding.Compilation.Api.Model;
using GeekCoding.Compilation.Execution;
using Microsoft.AspNetCore.Mvc;

namespace GeekCoding.Compilation.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/Execution")]
    public class ExecutionController : Controller
    {
        private IExecutionFile _executeFile;

        public ExecutionController(IExecutionFile executeFile)
        {
            _executeFile = executeFile;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "exec1LuciCB2", "exec2LuciCC3" };
        }

        [HttpPost]
        // POST api/<controller>
        public string Post([FromBody]ExecutionModel item)
        {
            if (ModelState.IsValid)
            {
                string executionRespone = _executeFile.Execute(item.ProblemName, item.UserName, "C++", item.TimeLimit, item.MemoryLimit);
                return executionRespone;
            }

            return "Failed response";
        }
    }
}