using GeekCoding.Data.Models;
using GeekCoding.MainApplication.Utilities.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekCoding.MainApplication.ViewModels
{
    public class ContestProblemViewModel
    {
        public ContestProblemViewModel()
        {
            ProblemId = new List<Guid>();
            Problems = new List<ProblemDto>();
            ProblemsForCurrentContest = new List<ProblemDto>();
        }
        public List<Guid> ProblemId { get; set; }
        public List<ProblemDto> Problems { get; set; } 
        public Guid ContestId { get; set; }
        public string ContestName { get; set; }
        public List<ProblemDto> ProblemsForCurrentContest { get; set; }
    }
}
