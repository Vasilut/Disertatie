using System;
using System.Collections.Generic;
using System.Text;

namespace GeekCoding.Data.Models
{
    public partial class Tests
    {
        public Guid TestId { get; set; }
        public int TestNumber { get; set; }
        public Guid ProblemId { get; set; }
        public string FisierIn { get; set; }
        public string FisierOk { get; set; }
        public string TestInput { get; set; }
        public string TestOutput { get; set; }
        public int Scor { get; set; }

        public Problem Problem { get; set; }
    }
}
