using GeekCoding.Data.Models;
using GeekCoding.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekCoding.Repository
{
    public class TestsRepository : RepositoryBase<Tests>, ITestsRepository
    {
        public TestsRepository(EvaluatorContext db):base(db)
        {

        }

        public override async Task<ICollection<Tests> > GetAllAsync()
        {
            return await RepositoryContext.Tests.Include(tst => tst.Problem).ToListAsync();
        }

        public override Tests GetItem(Guid id)
        {
            return RepositoryContext.Tests.Where(tst => tst.TestId == id).Include(tst => tst.Problem).FirstOrDefault();
        }

        public ICollection<Tests> GetTestsByProblemId(Guid problemId)
        {
            return RepositoryContext.Tests.Include(tst => tst.Problem).Where(tst => tst.ProblemId == problemId).ToList();
        }

        public int GetNumberOfTestForProblem(Guid problemId)
        {
            return RepositoryContext.Tests.Where(tst => tst.ProblemId == problemId).FirstOrDefault().TestNumber;
        }
    }
}
