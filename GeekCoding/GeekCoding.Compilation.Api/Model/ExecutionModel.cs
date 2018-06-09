using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekCoding.Compilation.Api.Model
{
    public class ExecutionModel
    {
        public string TimeLimit { get; set; }
        public string MemoryLimit { get; set; }
        
        public string ProblemName { get; set; }
        public string UserName { get; set; }
    }
}
