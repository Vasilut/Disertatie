using System;
using System.Collections.Generic;
using System.Text;

namespace GeekCoding.Common
{
    public interface IFileGenerator
    {
        void GenerateFile(string content, string language,
                                string problemName, string userName);
        string GetCurrentDirectory();

        string GetFileFullName(string problemName, string userName);
    }
}
