using GeekCoding.Compilation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GeekCoding
{
    public class DebugLibrary
    {
        static void Main(string[] args)
        {

            string file = @"D:\Boian Nebun-20180329T111622Z-001\Boian Nebun\JavaProjects\src\Program.java";
            string text = File.ReadAllText(file);

            ICompilationFile compilationFile = new CompilationFile();
            var compileResult = compilationFile.CompileFile(text, "Java", "javaloca", "boto");



            Console.WriteLine($"Verdict status: {compileResult.Item1}");
            Console.ReadLine();
        }
    }
}
