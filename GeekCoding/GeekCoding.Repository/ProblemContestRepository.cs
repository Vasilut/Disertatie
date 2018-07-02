using GeekCoding.Data.Models;
using GeekCoding.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeekCoding.Repository
{
    public class ProblemContestRepository : RepositoryBase<ProblemContest>, IProblemContestRepository
    {
        public ProblemContestRepository(EvaluatorContext db):base(db)
        {

        }
    }
}
