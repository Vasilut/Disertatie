using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekCoding.MainApplication.ViewModels
{
    public class Compilator
    {
        public static List<SelectListItem> Compilers
        {
            get
            {
                return new List<SelectListItem>()
                {
                    new SelectListItem {Text="C++",Value="C++"},
                    new SelectListItem {Text="Java",Value="Java"},
                    new SelectListItem {Text="C#",Value="C#"},
                    new SelectListItem {Text="py",Value="py"},
                    
                };
            }
        }
    };
}
