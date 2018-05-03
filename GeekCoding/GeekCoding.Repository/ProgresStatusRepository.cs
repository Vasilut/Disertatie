using GeekCoding.Data.Models;
using GeekCoding.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeekCoding.Repository
{
    public class ProgresStatusRepository : RepositoryBase<ProgresStatus>, IProgressStatusRepository
    {
        public ProgresStatusRepository(EvaluatorContext db) : base(db)
        {

        }
    }
}
