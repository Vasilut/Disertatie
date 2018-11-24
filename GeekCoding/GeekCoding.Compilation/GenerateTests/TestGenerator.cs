using GeekCoding.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeekCoding.Compilation.GenerateTests
{
    public class TestGenerator : ITestGenerator
    {
        private IFileGenerator _fileGenerator;

        public TestGenerator(IFileGenerator fileGenerator)
        {
            _fileGenerator = fileGenerator;
        }

        public bool DeleteTestFile(string fileInput, string fileOk, string problem)
        {
            return _fileGenerator.DeleteFile(fileInput, fileOk, problem);
        }

        public void GenerateFile(string file, string content, string problem)
        {
            _fileGenerator.GenerateTestFile(file, problem, content);
        }
    }
}
