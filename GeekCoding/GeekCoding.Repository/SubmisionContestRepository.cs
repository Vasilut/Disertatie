using GeekCoding.Data.Models;
using GeekCoding.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeekCoding.Repository
{
    public class SubmisionContestRepository : RepositoryBase<SubmisionContest>, ISubmisionContestRepository
    {
        public SubmisionContestRepository(EvaluatorContext db):base(db)
        {

        }
        public IEnumerable<SubmisionContest> GetListOfSubmisionForSpecificContest(Guid contestId)
        {
            return RepositoryContext.SubmisionContest.Include(x => x.Submision).Where(x => x.ContestId == contestId);
        }
    }
}
