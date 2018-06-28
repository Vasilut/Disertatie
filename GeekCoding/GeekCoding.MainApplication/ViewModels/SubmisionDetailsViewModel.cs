using GeekCoding.MainApplication.Utilities.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekCoding.MainApplication.ViewModels
{
    public class SubmisionDetailsViewModel
    {
        public SubmisionDetailsViewModel()
        {
            EvaluationResult = new List<TestModelDto>();
        }
        public string UserName { get; set; }
        public DateTime Data { get; set; }
        public string ProblemName { get; set; }
        public string Status { get; set; }
        public string Compilator { get; set; }
        public string SourceSize { get; set; }
        public string ProblemTimeLimit { get; set; }
        public string ProblemMemoryLimit { get; set; }
        public int Scor { get; set; }
        public List<TestModelDto> EvaluationResult { get; set; }
        public string MessageOfSubmission { get; set; }
    }
}
