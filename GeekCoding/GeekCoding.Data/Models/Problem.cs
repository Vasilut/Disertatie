using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeekCoding.Data.Models
{
    public partial class Problem
    {
        public Problem()
        {
            ProgresStatus = new HashSet<ProgresStatus>();
            Solution = new HashSet<Solution>();
            Submision = new HashSet<Submision>();
            Tests = new HashSet<Tests>();
            ProblemContest = new HashSet<ProblemContest>();
        }

        public Guid ProblemId { get; set; }
        public string ProblemName { get; set; }
        public string ProblemContent { get; set; }
        public string Dificulty { get; set; }
        public int GoodSubmision { get; set; }
        public int BadSubmission { get; set; }
        public string MemoryLimit { get; set; }
        public string TimeLimit { get; set; }
        public bool Visible { get; set; }
        [NotMapped]
        public double AverageAcceptance { get; set; }
        public ICollection<ProblemContest> ProblemContest { get; set; }

        public ICollection<ProgresStatus> ProgresStatus { get; set; }
        public ICollection<Solution> Solution { get; set; }
        public ICollection<Submision> Submision { get; set; }
        public ICollection<Tests> Tests { get; set; }
    }
}
