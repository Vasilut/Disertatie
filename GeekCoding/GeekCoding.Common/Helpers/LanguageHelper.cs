using System;
using System.Collections.Generic;
using System.Text;

namespace GeekCoding.Common.Helpers
{
    public class LanguageHelper
    {

        //compile under linux
        /*
         * c# ==> mcs -out:prob.exe Prob.cs
         * c++ ==> g++ -Wall -o prog Prob.cpp
         * java ==> javac Prob.java
         * python ==> python program.py
         */
        private static string[] CompileCommands = { "g++", "javac", "mcs", "python" };
        private static string[] LanguageExtension = { ".cpp", ".java", ".cs", ".py" };
        private static string[] LanguageExecutable = { ".exe", ".py" };
        private static string[] SandboxOperation = { "--init", "--cleanup" };

        public static string GetLanguageCompileCommand(string language, string fileToCompile,
                                                       string fileToExecute)
        {
            switch (language)
            {
                case "C++":
                    {
                        return $"{CompileCommands[0]} -Wall -o {fileToExecute} {fileToCompile}";
                    }
                case "Java":
                    {
                        return $"{CompileCommands[1]} {fileToCompile}";
                    }
                case "C#":
                    {
                        return $"{CompileCommands[2]} -out:{fileToExecute} {fileToCompile}";
                    }
                case "py":
                    {
                        return $"{CompileCommands[3]} {fileToCompile}";
                    }
                default:
                    break;
            }
            return string.Empty;
        }

        public static string GetLanguageExtenstionType(string language)
        {
            switch (language)
            {
                case "C++":
                    {
                        return LanguageExtension[0];
                    }
                case "Java":
                    {
                        return LanguageExtension[1];
                    }
                case "C#":
                    {
                        return LanguageExtension[2];
                    }
                case "py":
                    {
                        return LanguageExtension[3];
                    }
                default:
                    break;
            }
            return string.Empty;
        }

        public static string GetLanguageExecutableType(string language)
        {
            switch (language)
            {
                case "C++":
                    {
                        return LanguageExecutable[0];
                    }
                case "Java":
                    {
                        return string.Empty;
                    }
                case "C#":
                    {
                        return LanguageExecutable[0];
                    }
                case "py":
                    {
                        return LanguageExecutable[1];
                    }
                default:
                    break;
            }
            return string.Empty;
        }

        public static string GetSandboxOperation(string operation)
        {
            switch (operation)
            {
                case "INIT":
                    {
                        return $"./isolate {SandboxOperation[0]}";
                    }
                case "CLEAN":
                    {
                        return $"./isolate {SandboxOperation[1]}";
                    }
                default:
                    break;
            }
            return string.Empty;
        }

        public static string SandboxArguments(string timeLimit, string memoryLimit, string resultFile, string fileToExecute, string inputFile, string outputFile)
        {
            //result file = /tmp/logo3.txt
            //memory in kb (for example 4600)
            //time 1.5 seconds
            return $"./isolate --cg --meta={resultFile} --stdin={inputFile} --stdout={outputFile} --cg-mem={memoryLimit} --time={timeLimit} --run -- {fileToExecute}";
        }
    }
}
