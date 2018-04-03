using System;
using System.Collections.Generic;
using System.Text;

namespace GeekCoding.Common.Helpers
{
    public class Builder
    {
        public static string BuildProblemName(string problemName, string userName) => new StringBuilder(problemName).Append(userName).ToString();
    }
}
