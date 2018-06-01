using GeekCoding.Common;
using GeekCoding.Common.Helpers;
using GeekCoding.Common.ProcesExecuter;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeekCoding.Compilation
{
    public class CompilationFile : ICompilationFile
    {
        private IFileGenerator _fileGenerator;
        public CompilationFile()
        {
            _fileGenerator = new FileGenerator();
        }
        public Tuple<Verdict, string> CompileFile(string content, string language, string problemName, string userName)
        {
            //first we need to create the file
            _fileGenerator.GenerateFile(content, language,
                                        problemName, userName);

            //get files path
            string fullFilePath = _fileGenerator.GetFileFullName(problemName, userName, language);
            string fullFileToCompile = new StringBuilder(fullFilePath).Append(LanguageHelper.GetLanguageExtenstionType(language)).ToString();
            string fullFileExecutable = new StringBuilder(fullFilePath).Append(LanguageHelper.GetLanguageExecutableType(language)).ToString();
            string workingDirectory = _fileGenerator.GetCurrentDirectory();

            string argument = LanguageHelper.GetLanguageCompileCommand(language, fullFileToCompile, fullFileExecutable);
            var compilationProcess = ExternalProcessCompileExecuter.Instance;
            return compilationProcess.GetVerdictForFile(argument, workingDirectory);
        }
    }
}
