using System;
using System.Collections.Generic;
using System.Text;

namespace GeekCoding.Common.Helpers
{
    public class LanguageHelper
    {
        private static string[] CompileCommands = { "g++", "javac", "csc" };
        private static string[] LanguageExtension = { ".cpp", ".java", ".cs" };
        private static string[] LanguageExecutable = { ".exe" };

        public static string GetLanguageCompileCommand(string language, string fileToCompile,
                                                       string fileToExecute)
        {
            switch (language)
            {
                case "C++":
                    {
                        return $"{CompileCommands[0]} -o {fileToExecute} {fileToCompile}";
                    }
                case "Java":
                    {
                        return $"{CompileCommands[1]} {fileToCompile}";
                    }
                case "C#":
                    {
                        return $"{CompileCommands[2]} -out:{fileToExecute} {fileToCompile}";
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
                default:
                    break;
            }
            return string.Empty;
        }
    }
}
