using System;
using System.Collections.Generic;
using System.Text;

namespace GeekCoding.Data.Models
{
    public partial class ProblemContest
    {
        public Guid ProblemContestId { get; set; }
        public Guid ContestId { get; set; }
        public Guid ProblemId { get; set; }

        public Contest Contest { get; set; }
        public Problem Problem { get; set; }
    }
}
