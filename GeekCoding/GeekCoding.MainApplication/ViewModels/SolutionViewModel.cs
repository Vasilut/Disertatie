using GeekCoding.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekCoding.MainApplication.ViewModels
{
    public class SolutionViewModel
    {
        public Solution Solution { get; set; }
        public Guid ProblemId { get; set; }
        public string ProblemName { get; set; }
    }
}
