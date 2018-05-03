using System;
using System.Collections.Generic;

namespace GeekCoding.Data.Models
{
    public partial class ProgresStatus
    {
        public Guid ProgresStatusId { get; set; }
        public string UserName { get; set; }
        public Guid ProblemId { get; set; }
        public bool Status { get; set; }

        public Problem Problem { get; set; }
    }
}
