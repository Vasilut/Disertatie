using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GeekCoding.MainApplication.ViewModels
{
    public class TestsViewModel
    {
        public Guid TestId { get; set; }
        public int TestNumber { get; set; }
        public Guid ProblemId { get; set; }
        public string FisierIn { get; set; }
        public string FisierOk { get; set; }
        public string TestInput { get; set; }
        public string TestOutput { get; set; }
        public int Scor { get; set; }
        [Required]
        [Display(Name = "fileInput")]
        public IFormFile FileInput { get; set; }
        [Required]
        [Display(Name = "fileOutput")]
        public IFormFile FileOutput { get; set; }
    }
}
