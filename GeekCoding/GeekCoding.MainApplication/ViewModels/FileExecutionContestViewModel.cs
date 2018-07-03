using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GeekCoding.MainApplication.ViewModels
{
    public class FileExecutionContestViewModel
    {
        [Required]
        [Display(Name = "file")]
        public IFormFile File { get; set; }
        public string Compilator { get; set; }
        public string ProblemId { get; set; }
        public string ProblemName { get; set; }
        public Guid ContestId { get; set; }
    }
}
