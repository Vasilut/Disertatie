using GeekCoding.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeekCoding.Repository.Interfaces
{
    public interface ISubmisionContestRepository : IRepositoryBase<SubmisionContest>
    {
        IEnumerable<SubmisionContest> GetListOfSubmisionForSpecificContest(Guid contestId);
    }
}
