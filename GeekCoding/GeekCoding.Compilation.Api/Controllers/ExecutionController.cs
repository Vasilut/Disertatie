using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace GeekCoding.Compilation.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/Execution")]
    public class ExecutionController : Controller
    {
        public ExecutionController()
        {

        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "exec1", "exec2" };
        }
    }
}