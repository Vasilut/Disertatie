using System;
using System.Collections.Generic;

namespace GeekCoding.Data.Models
{
    public partial class Problem
    {
        public Problem()
        {
            ProgresStatus = new HashSet<ProgresStatus>();
            Solution = new HashSet<Solution>();
            Submision = new HashSet<Submision>();
        }

        public Guid ProblemId { get; set; }
        public string ProblemName { get; set; }
        public string ProblemContent { get; set; }
        public string Dificulty { get; set; }
        public int GoodSubmision { get; set; }
        public int BadSubmission { get; set; }
        public bool Visible { get; set; }

        public ICollection<ProgresStatus> ProgresStatus { get; set; }
        public ICollection<Solution> Solution { get; set; }
        public ICollection<Submision> Submision { get; set; }
    }
}
