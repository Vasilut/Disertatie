using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeekCoding.Data.Models
{
    public partial class Submision
    {
        public Submision()
        {
            Evaluation = new HashSet<Evaluation>();
            SubmisionContest = new HashSet<SubmisionContest>();
        }
        public Guid SubmisionId { get; set; }
        public Guid ProblemId { get; set; }
        public string UserName { get; set; }
        public DateTime DataOfSubmision { get; set; }
        public string SourceSize { get; set; }
        public int Score { get; set; }
        public string StateOfSubmision { get; set; }
        public string Compilator { get; set; }
        public string MessageOfSubmision { get; set; }
        [NotMapped]
        public string SourceLocation { get; set; }
        public Problem Problem { get; set; }
        public bool? JobQueued { get; set; }
        public string SourceCode { get; set; }
        public ICollection<Evaluation> Evaluation { get; set; }
        public ICollection<SubmisionContest> SubmisionContest { get; set; }

    }
}
