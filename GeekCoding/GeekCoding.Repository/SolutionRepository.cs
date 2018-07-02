using GeekCoding.Data.Models;
using GeekCoding.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeekCoding.Repository
{
    public class SolutionRepository : RepositoryBase<Solution>, ISolutionRepository
    {
        public SolutionRepository(EvaluatorContext db) : base(db)
        {

        }
        public override Solution GetItem(Guid id)
        {
            return RepositoryContext.Solution.Where(x => x.SolutionId == id).Include(x => x.Problem).FirstOrDefault();
        }

        public Solution GetSolutionByProblem(Guid problemId)
        {
            return RepositoryContext.Solution.Include(sol => sol.Problem).Where(sol => sol.ProblemId == problemId).FirstOrDefault();
        }
    }
}
