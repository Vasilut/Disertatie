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
        public ExecutionFile()
        {
            _fileGenerator = new FileGenerator();
        }
        
        public List<Tuple<string,string>> Execute(string problemName, string userName, string language,
                                            string timeLimit, string memoryLimit, string testFile, int numbefOfTests)
        {
            List<Tuple<string, string>> _executionResult;  _executionResult = new List<Tuple<string, string>>();  
            string sourcesDirectory = _fileGenerator.GetCurrentDirectory();  // /usr/local/etc/sources           
            string fileName = Builder.BuildProblemName(problemName, userName); // Codarelucian.vasilut10@gmail.com
            string fullFileName = _fileGenerator.GetFileFullName(problemName, userName); // /usr/local/etc/sources/Codarelucian.vasilut10@gmail.com
            string fullFileExecutable = new StringBuilder(fullFileName).Append(LanguageHelper.GetLanguageExecutableType(language)).ToString(); // /usr/local/etc/sources/Codarelucian.vasilut10@gmail.exe
            string fileToExecute = new StringBuilder(fileName).Append(LanguageHelper.GetLanguageExecutableType(language)).ToString(); // Codarelucian.vasilut10@gmail.com.exe
            
            string inputFileFolder = _fileGenerator.BuildNewDirectory(sourcesDirectory, problemName); //usr/local/etc/sources/minge

            //we have next steps:
            /*
             * 1. initialize the sandbox. ./isolate --init
             * 2. copy the file in sandbox environment: cp /usr/local/etc/sources/file /tmp/box/0/box/
             * 3. copy the "test.in" files in sandbox environment
             * 4. run the file in sanxbox: ./isolate --cg --meta=/tmp/result.txt --cg-mem=5000 --time=1.5 --run -- program
             * ./isolate --cg --meta=/tmp/muc2.txt --stdin=test1.in --stdout=fis1.out --cg-mem=3000 --time=2 --run -- prog  --> run over all the tests this will be ran
             * steps 3 and 4  and 5 need to be ran over all the tests.
             * 5. run sh script
             * 6. clean sandbox: ./isolate --clean
             */
            var executionProcess = ExternalProcessCompileExecuter.Instance;

            //step 1
            string initArgument = LanguageHelper.GetSandboxOperation(_sandboxOperations[0]);
            executionProcess.SandboxOperation(initArgument, _isolateDirectory);

            //step 2
            string copyArgument = $"cp -a {fullFileExecutable} {_sandboxDirectory}";
            executionProcess.SandboxOperation(copyArgument, _homeDirectory);

            for(int i = 0; i < numbefOfTests; ++i)
            {
                int testNumber = i + 1;
                string inputFileName = $"{testFile}{testNumber}.in"; //minge1.in for example
                string outputFileName = $"{testFile}{testNumber}.ok"; //minge1.ok for example
                string outputSandboxFileName = $"{testFile}{testNumber}.out"; //in sandbox o sa fie generat minge1.out de exemplu

                string inputFileNamePath = Path.Combine(inputFileFolder, inputFileName);
                string outputFileNamePath = Path.Combine(inputFileFolder, outputFileName);
                string outputSandboxFileNamePath = Path.Combine(_sandboxDirectory, outputSandboxFileName);

                //step 3
                copyArgument = $"cp -a {inputFileNamePath} {_sandboxDirectory}"; //copy input file to sandbox
                executionProcess.SandboxOperation(copyArgument, _homeDirectory);

                //step 4
                string executionArgument = LanguageHelper.SandboxArguments(timeLimit, memoryLimit, "/tmp/results.txt", fileToExecute, inputFileName, outputSandboxFileName, _sandboxDirectory);
                executionProcess.SandboxOperation(executionArgument, _isolateDirectory);
                string executionResult = _fileGenerator.ReadExectutionResult();

                //step 5 run bash script to compare the ok file with the generated one to see the results.
                string argumentBash = $"sh prog.sh {outputSandboxFileNamePath} {outputFileNamePath}";
                string executionResponse = executionProcess.SandboxOperation(argumentBash, sourcesDirectory);

                _executionResult.Add(new Tuple<string, string>(executionResult, executionResponse));
            }

            //step 6
            string cleanArgument = LanguageHelper.GetSandboxOperation(_sandboxOperations[1]);
            executionProcess.SandboxOperation(cleanArgument, _isolateDirectory);

            return _executionResult;

        }
        
    }
}
