using GeekCoding.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekCoding.MainApplication.ViewModels
{
    public class TestViewModel
    {
        public TestViewModel()
        {
            ListModel = new List<Tests>();
        }
        public List<Tests> ListModel { get; set; }
        public Guid ProblemId { get; set; }
    }
}
