using System;
using System.Collections.Generic;
using System.Text;

namespace GeekCoding.Data.Models
{
    public partial class UserContest
    {
        public Guid UserContestId { get; set; }
        public string UserName { get; set; }
        public Guid ContestId { get; set; }

        public Contest Contest { get; set; }
    }
}
