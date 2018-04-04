using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekCoding.Compilation.Api.Model
{
    public class CompilationModel
    {
        public string Content { get; set; }
        public string Language { get; set; }
        public string ProblemName { get; set; }
        public string Username { get; set; }
    }
}
