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
        private const string _sandboxDirectory = "/tmp/box/0/box/";
        private string[] _sandboxOperations = { "INIT", "CLEAN" };
        private const string _homeDirectory = "/home";
        private const string _inputFile = "fisier.in";
        private const string _outputFile = "fis1.out";
        private const string _outputFileOk = "fis1-ok.out";
        public ExecutionFile()
        {
            _fileGenerator = new FileGenerator();
        }
        
        public Tuple<string,string> Execute(string problemName, string userName, string language, string timeLimit, string memoryLimit)
        {
            string sourcesDirectory = _fileGenerator.GetCurrentDirectory();
            string fileName = Builder.BuildProblemName(problemName, userName);
            string fullFileName = _fileGenerator.GetFileFullName(problemName, userName);
            string fullFileExecutable = new StringBuilder(fullFileName).Append(LanguageHelper.GetLanguageExecutableType(language)).ToString();
            string fileToExecute = new StringBuilder(fileName).Append(LanguageHelper.GetLanguageExecutableType(language)).ToString();

            string inputFilePath = Path.Combine(sourcesDirectory, _inputFile);
            string outFileGeneratedPath = Path.Combine(_sandboxDirectory, _outputFile);
            string outFileOkPath = Path.Combine(sourcesDirectory, _outputFileOk);

            //we have next steps:
            /*
             * 1. initialize the sandbox. ./isolate --init
             * 2. copy the file in sandbox environment: cp /usr/local/etc/sources/file /tmp/box/0/box/
             * 3. copy the "test.in" files in sandbox environment
             * 4. run the file in sanxbox: ./isolate --cg --meta=/tmp/result.txt --cg-mem=5000 --time=1.5 --run -- program
             * ./isolate --cg --meta=/tmp/muc2.txt --stdin=test1.in --stdout=fis1.out --cg-mem=3000 --time=2 --run -- prog  --> run over all the tests this will be ran
             * steps 3 and 4 need to be ran over all the tests.
             * 5. clean sandbox: ./isolate --clean
             */
            var executionProcess = ExternalProcessCompileExecuter.Instance;

            //step 1
            string initArgument = LanguageHelper.GetSandboxOperation(_sandboxOperations[0]);
            executionProcess.SandboxOperation(initArgument, _isolateDirectory);

            //step 2
            string copyArgument = $"cp -a {fullFileExecutable} {_sandboxDirectory}";
            executionProcess.SandboxOperation(copyArgument, _homeDirectory);

            //step 3
            copyArgument = $"cp -a {inputFilePath} {_sandboxDirectory}";
            executionProcess.SandboxOperation(copyArgument, _homeDirectory);

            //step 4
            string executionArgument = LanguageHelper.SandboxArguments(timeLimit, memoryLimit, "/tmp/results.txt", fileToExecute, _inputFile, _outputFile);
            executionProcess.SandboxOperation(executionArgument, _isolateDirectory);
            string executionResult = _fileGenerator.ReadExectutionResult();

            //step 5run bash script to compare the ok file with the generated one to see the results.
            string argumentBash = $"sh prog.sh {outFileGeneratedPath} {outFileOkPath}";
            string executionResponse = executionProcess.SandboxOperation(argumentBash, sourcesDirectory);


            //step 6
            string cleanArgument = LanguageHelper.GetSandboxOperation(_sandboxOperations[1]);
            executionProcess.SandboxOperation(cleanArgument, _isolateDirectory);

            return new Tuple<string,string>(executionResult, executionResponse);

        }
    }
}
