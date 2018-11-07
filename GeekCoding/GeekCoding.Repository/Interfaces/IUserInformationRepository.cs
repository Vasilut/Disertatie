using GeekCoding.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeekCoding.Repository.Interfaces
{
    public interface IUserInformationRepository : IRepositoryBase<UserInformation>
    {
        UserInformation GetUserInformationByUsername(string username);
    }
}
