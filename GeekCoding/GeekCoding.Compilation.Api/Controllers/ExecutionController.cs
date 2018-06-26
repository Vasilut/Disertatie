using System;
using System.Collections.Generic;
using System.Text;
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
            return new string[] { "exec1LuciBlana", "exec2LuciTareDetot" };
        }

        [HttpPost]
        // POST api/<controller>
        public string Post([FromBody]ExecutionModel item)
        {
            if (ModelState.IsValid)
            {
                Tuple<string,string> executionResponse = _executeFile.Execute(item.ProblemName, item.UserName, "C++", item.TimeLimit, item.MemoryLimit);
                return new StringBuilder(executionResponse.Item1).Append(" ").Append("Response: ").Append(executionResponse.Item2).ToString();
            }

            return "Failed response";
        }
    }
}