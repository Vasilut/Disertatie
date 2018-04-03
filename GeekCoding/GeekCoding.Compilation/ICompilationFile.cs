using GeekCoding.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeekCoding.Compilation
{
    public interface ICompilationFile
    {
        Tuple<Verdict, string> CompileFile(string content, string language,
                                          string problemName, string userName);
    }
}
