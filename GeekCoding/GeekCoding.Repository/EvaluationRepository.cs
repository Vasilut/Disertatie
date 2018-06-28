using GeekCoding.Data.Models;
using GeekCoding.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace GeekCoding.Repository
{
    public class EvaluationRepository : RepositoryBase<Evaluation>, IEvaluationRepository
    {
        public EvaluationRepository(EvaluatorContext db):base(db)
        {

        }

        public override async Task<Evaluation> GetAsync(Guid id)
        {
            return await RepositoryContext.Evaluation.Where(x => x.EvaluationId == id).Include(x => x.Submision).FirstOrDefaultAsync();
        }

        public override Evaluation GetItem(Guid id)
        {
            return RepositoryContext.Evaluation.Where(x => x.EvaluationId == id).Include(x => x.Submision).FirstOrDefault();
        }

        public Evaluation GetItemBySubmission(Guid submissionId)
        {
            return RepositoryContext.Evaluation.Include(x => x.Submision).Where(x => x.SubmisionId == submissionId).FirstOrDefault();
        }
    }
}
