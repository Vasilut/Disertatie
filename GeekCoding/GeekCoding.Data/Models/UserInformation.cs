using System;
using System.Collections.Generic;

namespace GeekCoding.Data.Models
{
    public partial class UserInformation
    {
        public string IdUser { get; set; }
        public string Nume { get; set; }
        public string Prenume { get; set; }
        public string Profesor { get; set; }
        public string Clasa { get; set; }
        public string Scoala { get; set; }
        public string Username { get; set; }

        public AspNetUsers IdUserNavigation { get; set; }
    }
}
