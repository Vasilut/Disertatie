using GeekCoding.Data.Models;
using GeekCoding.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeekCoding.Repository
{
    public class ContestRepository : RepositoryBase<Contest>, IContestRepository
    {
        public ContestRepository(EvaluatorContext db):base(db)
        {

        }
    }
}
