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
    public class SubmisionRepository : RepositoryBase<Submision>, ISubmisionRepository
    {
        public SubmisionRepository(EvaluatorContext db) : base(db)
        {

        }

        public override async Task<Submision> GetAsync(Guid id)
        {
            return await RepositoryContext.Submision.Where(x => x.SubmisionId == id).Include(x => x.Problem).FirstOrDefaultAsync();
        }
    }
}
