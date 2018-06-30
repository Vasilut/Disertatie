using System;
using System.Collections.Generic;
using System.Text;

namespace GeekCoding.Compilation.GenerateTests
{
    public interface ITestGenerator
    {
        void GenerateFile(string file, string content, string problem);
    }
}
