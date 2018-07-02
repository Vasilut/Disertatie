using System;
using System.Collections.Generic;
using System.Text;

namespace GeekCoding.Data.Models
{
    public partial class SubmisionContest
    {
        public Guid SubmisionContestId { get; set; }
        public Guid ContestId { get; set; }
        public Guid SubmisionId { get; set; }

        public Contest Contest { get; set; }
        public Submision Submision { get; set; }
    }
}
