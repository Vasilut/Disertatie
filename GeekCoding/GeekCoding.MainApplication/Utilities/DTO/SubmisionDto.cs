using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekCoding.MainApplication.Utilities
{
    public class SubmisionDto
    {
        public Guid SubmissionId { get; set; }
        public string ProblemName { get; set; }
        public string UserName { get; set; }
        public string Content { get; set; }
        public string Compilator { get; set; }
        public string MemoryLimit { get; set; }
        public string TimeLimit { get; set; }

        
    }
}
