using GeekCoding.Data.Models;
using GeekCoding.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeekCoding.Repository
{
    public class SolutionRepository : RepositoryBase<Solution>, ISolutionRepository
    {
        public SolutionRepository(EvaluatorContext db) : base(db)
        {

        }
    }
}
