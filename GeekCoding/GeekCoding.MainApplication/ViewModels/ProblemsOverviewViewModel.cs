using GeekCoding.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekCoding.MainApplication.ViewModels
{
    public class ProblemsOverviewViewModel
    {
        public ProblemsOverviewViewModel()
        {
            ContestProblemList = new List<Problem>();
        }
        public List<Problem> ContestProblemList { get; set; }
        public Guid ContestId { get; set; }
    }
}
