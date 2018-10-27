using GeekCoding.Data.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekCoding.MainApplication.ViewModels
{
    public class ProblemContestDetailsViewModel
    {
        public Problem Problem { get; set; }
        public List<SelectListItem> SelectListItems { get; set; }
        public int Score { get; set; }
        public Guid ContestId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
