using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeekCoding.Compilation.Api.Model;

namespace GeekCoding.MainApplication.Utilities
{
    public interface ISerializeTests
    {
        Tuple<string,int> SerializeReponseTest(List<ResponseExecutionModel> responseExecutionModels);
    }
}
