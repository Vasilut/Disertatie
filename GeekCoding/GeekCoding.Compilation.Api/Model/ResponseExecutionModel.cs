using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekCoding.Compilation.Api.Model
{
    [Serializable]
    public class ResponseExecutionModel
    {
        public string ExecutionStatus { get; set; }
        public string ExecutionResults { get; set; }
    }
}
