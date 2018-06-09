using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace GeekCoding.MainApplication.ViewModels
{
    public class FileExecutionViewModel
    {
        [Required]
        [Display(Name = "file")]
        public IFormFile File { get; set; }
        public string Compilator { get; set; }
        public string ProblemId { get; set; }
        public string ProblemName { get; set; }
    }
}
