using System;
using System.Collections.Generic;
using System.Text;
using GeekCoding.Compilation.Api.Model;
using GeekCoding.Compilation.Execution;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
            return new string[] { "exec1LuciCelMaiTareTareDragnea222", "exec2LuciTareFelinaBlanaGoodDancil22a2" };
        }

        [HttpPost]
        // POST api/<controller>
        public JsonResult Post([FromBody]ExecutionModel item)
        {
            if (ModelState.IsValid)
            {
                var lstExecutionResponse = _executeFile.Execute(item.ProblemName, item.UserName, item.Compilator,
                                                                              item.TimeLimit, item.MemoryLimit, item.FileName, item.NumberOfTests);
                var lst = new List<ResponseExecutionModel>();
                foreach (var itemRsp in lstExecutionResponse)
                {
                    lst.Add(new ResponseExecutionModel
                    {
                        ExecutionResults = itemRsp.Item1,
                        ExecutionStatus = itemRsp.Item2
                    });
                }
                return Json(lst);
            }

            return Json("Failed response");
        }
    }
}