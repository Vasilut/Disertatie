using GeekCoding.Data.Models;
using GeekCoding.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeekCoding.Repository
{
    public class SubmisionRepository : RepositoryBase<Submision>, ISubmisionRepository
    {
        public SubmisionRepository(EvaluatorContext db) : base(db)
        {

        }
    }
}
