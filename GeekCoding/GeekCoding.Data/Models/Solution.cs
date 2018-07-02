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

        public string Author { get; set; }
        public DateTime DateAdded { get; set; }
        public bool Visible { get; set; }
        public string Name { get; set; }
    }
}
