using GeekCoding.Data.Models;
using GeekCoding.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeekCoding.Repository
{
    public class UserInformationRepository : RepositoryBase<UserInformation>, IUserInformationRepository
    {
        public UserInformationRepository(EvaluatorContext repoContext) : base(repoContext)
        {
        }

        public UserInformation GetUserInformationByUsername(string username)
        {
            return RepositoryContext.UserInformation.Where(usrInf => usrInf.Username == username).FirstOrDefault();
        }
    }
}
