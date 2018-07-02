using GeekCoding.Data.Models;
using GeekCoding.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeekCoding.Repository
{
    public class ProblemContestRepository : RepositoryBase<ProblemContest>, IProblemContestRepository
    {
        public ProblemContestRepository(EvaluatorContext db):base(db)
        {

        }

         public IEnumerable<ProblemContest> GetListOfProblemForSpecificContest(Guid id)
        {
            return RepositoryContext.ProblemContest.Include(x => x.Problem).Include(x => x.Contest).Where(x => x.ContestId == id);
        }

        public override IQueryable<ProblemContest> GetAll()
        {
            return RepositoryContext.ProblemContest.Include(x => x.Problem).Include(x => x.Contest);
        }
    }
}
