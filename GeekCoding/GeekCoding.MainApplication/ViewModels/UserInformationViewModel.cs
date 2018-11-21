using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekCoding.MainApplication.ViewModels
{
    public class UserInformationViewModel
    {
        public string IdUser { get; set; }
        public string Nume { get; set; }
        public string Prenume { get; set; }
        public string Profesor { get; set; }
        public string Clasa { get; set; }
        public string Scoala { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public List<SelectListItem> SelectListItems { get; set; }
        public string Role { get; set; }
    }
}
