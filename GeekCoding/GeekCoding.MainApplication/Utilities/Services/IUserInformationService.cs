using GeekCoding.MainApplication.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekCoding.MainApplication.Utilities.Services
{
    public interface IUserInformationService
    {
        Task<bool> RegisterUser(UserInformationViewModel userInformation, string role);
    }
}
