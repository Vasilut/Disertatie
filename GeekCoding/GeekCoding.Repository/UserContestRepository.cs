using GeekCoding.Data.Models;
using GeekCoding.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeekCoding.Repository
{
    public class UserContestRepository : RepositoryBase<UserContest>, IUserContestRepository
    {
        public UserContestRepository(EvaluatorContext db):base(db)
        {

        }
    }
}
