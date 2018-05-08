using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekCoding.MainApplication.ViewModels
{
    public class ProblemViewModel
    {
        public string ProblemName { get; set; }
        public string ProblemContent { get; set; }
        public string Dificulty { get; set; }
        public bool Visible { get; set; }
    }
}
