using GeekCoding.Data.Models;
using GeekCoding.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeekCoding.Repository
{
    public class SubmisionContestRepository : RepositoryBase<SubmisionContest>, ISubmisionContestRepository
    {
        public SubmisionContestRepository(EvaluatorContext db):base(db)
        {

        }
    }
}
