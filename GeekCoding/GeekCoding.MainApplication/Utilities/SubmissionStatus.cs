using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekCoding.MainApplication.Utilities
{
    public enum SubmissionStatus
    {
        NotCompiled,
        Compiled,
        Compiling,
        CompilationError,
        Executed,
        ServerError
    }
}
