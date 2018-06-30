using GeekCoding.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeekCoding.Repository.Interfaces
{
    public interface ITestsRepository : IRepositoryBase<Tests>
    {
        ICollection<Tests> GetTestsByProblemId(Guid problemId);
    }
}
