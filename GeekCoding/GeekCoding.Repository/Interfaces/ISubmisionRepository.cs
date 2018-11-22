using GeekCoding.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeekCoding.Repository.Interfaces
{
    public interface ISubmisionRepository : IRepositoryBase<Submision>
    {
        IQueryable<Submision> GetSubmisionByProblemIdAndUserName(Guid problemId, string userName);
    }
}
