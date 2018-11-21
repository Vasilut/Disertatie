using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekCoding.MainApplication.ViewModels
{
    public class Membership
    {
        public static List<SelectListItem> Roles
        {
            get
            {
                return new List<SelectListItem>()
                {
                    new SelectListItem {Text="Admin",Value="Admin"},
                    new SelectListItem{ Text = "Member", Value = "Member" },
                    new SelectListItem{ Text = "Proponent", Value = "Proponent"}
                };
            }
        }
    }
}
