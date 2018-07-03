using GeekCoding.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekCoding.MainApplication.ViewModels
{
    public class UserContestViewModel
    {
        public Contest Contest { get; set; }
        public bool UserRegistered { get; set; }
    }
}
