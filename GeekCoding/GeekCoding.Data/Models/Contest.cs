using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GeekCoding.Data.Models
{
    public partial class Contest
    {
        public Contest()
        {
            Announcement = new HashSet<Announcement>();
            ProblemContest = new HashSet<ProblemContest>();
            SubmisionContest = new HashSet<SubmisionContest>();
            UserContest = new HashSet<UserContest>();
        }

        public Guid ContestId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string StatusContest { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [NotMapped]
        public double Duration { get; set; }

        public ICollection<Announcement> Announcement { get; set; }
        public ICollection<ProblemContest> ProblemContest { get; set; }
        public ICollection<SubmisionContest> SubmisionContest { get; set; }
        public ICollection<UserContest> UserContest { get; set; }
    }
}
