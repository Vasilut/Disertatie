using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekCoding.MainApplication.ViewModels
{
    public class RankingViewModel
    {
        public RankingViewModel()
        {
            ProblemList = new List<string>();
            Scores = new List<int>();
        }
        public string Participant { get; set; }
        public List<string> ProblemList { get; set; }
        public List<int> Scores { get; set; }
        public int Total { get; set; }
    }
}
