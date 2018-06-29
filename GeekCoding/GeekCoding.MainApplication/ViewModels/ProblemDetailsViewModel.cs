using GeekCoding.Data.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekCoding.MainApplication.ViewModels
{
    public class ProblemDetailsViewModel
    {
        public Problem Problem { get; set; }
        public List<SelectListItem> SelectListItems { get; set; }
        public List<Submision> Submisions { get; set; }
        public Solution Solution { get; set; }
    }
}
