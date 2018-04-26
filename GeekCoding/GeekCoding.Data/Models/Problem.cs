using System;
using System.Collections.Generic;

namespace GeekCoding.Data.Models
{
    public partial class Problem
    {
        public Guid ProblemId { get; set; }
        public string ProblemName { get; set; }
        public string ProblemContent { get; set; }
    }
}
