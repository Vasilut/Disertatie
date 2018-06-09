using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

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
