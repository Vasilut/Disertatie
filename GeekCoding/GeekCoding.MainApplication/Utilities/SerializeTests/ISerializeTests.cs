using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekCoding.MainApplication.Utilities
{
    public interface ISerializeTests
    {
        Tuple<string,int> SerializeReponseTest(List<string> tests);
    }
}
