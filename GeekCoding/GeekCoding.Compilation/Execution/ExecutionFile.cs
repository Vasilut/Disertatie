using GeekCoding.Common;
using GeekCoding.Common.Helpers;
using GeekCoding.Common.ProcesExecuter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GeekCoding.Compilation.Execution
{
    public class ExecutionFile : IExecutionFile
    {
        private IFileGenerator _fileGenerator;
        private const string _isolateDirectory = "/usr/local/bin";
        private const string _sandboxDirectory = "/tmp/box/0/box";
        private string[] _sandboxOperations = { "INIT", "CLEAN" };
        public ExecutionFile()
        {
            _fileGenerator = new FileGenerator();
        }
        
        public void Execute(string problemName, string userName, string language, string timeLimit, string memoryLimit)
        {
            string sourcesDirectory = _fileGenerator.GetCurrentDirectory();
            string fileName = Builder.BuildProblemName(problemName, userName);
            string fullFileName = _fileGenerator.GetFileFullName(problemName, userName);
            string fullFileExecutable = new StringBuilder(fullFileName).Append(LanguageHelper.GetLanguageExecutableType(language)).ToString();
            string fileToExecute = new StringBuilder(fileName).Append(LanguageHelper.GetLanguageExecutableType(language)).ToString();

            //we have next steps:
            /*
             * 1. initialize the sandbox. ./isolate --init
             * 2. copy the file in sandbox environment: cp /usr/local/etc/sources/file /tmp/box/0/box
             * 3. run the file in sanxbox: ./isolate --cg --meta=/tmp/result.txt --cg-mem=5000 --time=1.5 --run -- program
             * 4. clean sandbox: ./isolate --clean
             */
            var executionProcess = ExternalProcessCompileExecuter.Instance;

            //step 1
            string initArgument = LanguageHelper.GetSandboxOperation(_sandboxOperations[0]);
            executionProcess.SandboxOperation(initArgument, _isolateDirectory);

            //step 2
            string copyArgument = $"cp {fullFileExecutable} {_sandboxDirectory}";
            executionProcess.SandboxOperation(copyArgument, sourcesDirectory); //not sure if the directory is sourcesDirectory

            //step 3
            string executionArgument = LanguageHelper.SandboxArguments(timeLimit, memoryLimit, "/tmp/logogood.txt", fileToExecute);


            //step 4
            string cleanArgument = LanguageHelper.GetSandboxOperation(_sandboxOperations[1]);
            executionProcess.SandboxOperation(cleanArgument, _isolateDirectory);

        }
    }
}
