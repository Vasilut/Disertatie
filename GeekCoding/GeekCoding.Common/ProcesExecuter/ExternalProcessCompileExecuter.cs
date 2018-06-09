using GeekCoding.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GeekCoding.Common.ProcesExecuter
{
    //implement like singleton
    public sealed class ExternalProcessCompileExecuter
    {
        private static readonly ExternalProcessCompileExecuter instance = new ExternalProcessCompileExecuter();

        static ExternalProcessCompileExecuter()
        {

        }

        private ExternalProcessCompileExecuter()
        {

        }

        public static ExternalProcessCompileExecuter Instance
        {
            get
            {
                return instance;
            }
        }

        public Tuple<Verdict, string> GetVerdictForFile(string argument,string workingDirectory)
        {
            Process process = new Process();
            process.StartInfo.FileName = "/bin/bash";
            process.StartInfo.Arguments = $"-c \"{argument}\"";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.WorkingDirectory = workingDirectory;
            process.Start();
            //* Read the output (or the error)

            string output = process.StandardOutput.ReadToEnd();
            Console.WriteLine(output);
            string err = process.StandardError.ReadToEnd();
            Console.WriteLine(err);
            process.WaitForExit();
            process.Close();

            Verdict verdict = Verdict.SUCCESS;
            StringBuilder sb = new StringBuilder();

            if(!string.IsNullOrEmpty(err))
            {
                verdict = Verdict.ERROR;
                sb.Append("CompilationErrors: ").Append(err);
            }
            else
            {
                sb.Append("Output: ").Append(output);
            }
            
            return new Tuple<Verdict, string>(verdict, sb.ToString());

        }

        public void SandboxOperation(string argument, string workingDirectory)
        {
            Process process = new Process();
            process.StartInfo.FileName = "/bin/bash";
            process.StartInfo.Arguments = $"-c \"{argument}\"";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.WorkingDirectory = workingDirectory;
            process.Start();

            Console.WriteLine("");
            process.WaitForExit();
            process.Close();
        }
    }
}
