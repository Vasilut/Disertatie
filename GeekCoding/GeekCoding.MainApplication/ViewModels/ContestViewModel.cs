using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekCoding.MainApplication.ViewModels
{
    public class ContestViewModel
    {
        public Guid ContestId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string StatusContest { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
