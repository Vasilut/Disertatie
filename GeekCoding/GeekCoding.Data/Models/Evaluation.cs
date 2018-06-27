using System;
using System.Collections.Generic;

namespace GeekCoding.Data.Models
{
    public partial class Evaluation
    {
        public Guid EvaluationId { get; set; }
        public Guid SubmisionId { get; set; }
        public string EvaluationResult { get; set; }
        public int Score { get; set; }
        public Submision Submision { get; set; }
    }
}
