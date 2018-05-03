using System;
using System.Collections.Generic;

namespace GeekCoding.Data.Models
{
    public partial class Solution
    {
        public Guid SolutionId { get; set; }
        public Guid ProblemId { get; set; }
        public string Content { get; set; }

        public Problem Problem { get; set; }
    }
}
