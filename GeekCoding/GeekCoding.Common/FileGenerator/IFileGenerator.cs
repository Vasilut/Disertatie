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

        string ReadExectutionResult();
        void GenerateTestFile(string fileName, string problem, string content);
        bool DeleteFile(string fileInput, string fileOk, string problem);
        string BuildNewDirectory(string oldPath, string folderName);
    }
}
