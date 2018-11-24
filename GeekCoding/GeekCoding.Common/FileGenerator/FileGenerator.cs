using GeekCoding.Common.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GeekCoding.Common
{
    public class FileGenerator : IFileGenerator
    {
        private const string SOURCES = "sources";
        private const string ExecutionFileResult = "/tmp/results.txt";
        public void GenerateFile(string content, string language, string problemName, string userName)
        {
            //create directory
            var goodDirectory = GetCurrentDirectory();
            if (!Directory.Exists(goodDirectory))
            {
                Directory.CreateDirectory(goodDirectory);
            }

            StringBuilder sb = new StringBuilder();
            string problemFullName = Builder.BuildProblemName(problemName, userName);
            sb.Append(problemFullName).Append(LanguageHelper.GetLanguageExtenstionType(language));
            string sourceName = sb.ToString();

            //create file
            var fileToCreate = Path.Combine(goodDirectory, sourceName);
            if (File.Exists(fileToCreate))
            {
                File.Delete(fileToCreate);
            }
            if (!File.Exists(fileToCreate))
            {
                using (FileStream fs = new FileStream(fileToCreate, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                {
                    content = content.Trim();
                    StreamWriter writer = new StreamWriter(fs, Encoding.ASCII);
                    writer.Write(content);
                    writer.Flush();
                }
            }
        }

        public void GenerateTestFile(string fileName, string problem, string content)
        {
            string fileToCreate = GenerateFilesAndDirectory(fileName, problem);
            if (!File.Exists(fileToCreate))
            {
                using (FileStream fs = new FileStream(fileToCreate, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                {
                    content = content.Trim();
                    StreamWriter writer = new StreamWriter(fs, Encoding.ASCII);
                    writer.Write(content);
                    writer.Flush();
                }
            }

        }

        private string GenerateFilesAndDirectory(string fileName, string problem)
        {
            //create directory
            var goodDirectory = GetCurrentDirectory();
            if (!Directory.Exists(goodDirectory))
            {
                Directory.CreateDirectory(goodDirectory);
            }

            var testDirectory = Path.Combine(goodDirectory, problem);
            if (!Directory.Exists(testDirectory))
            {
                Directory.CreateDirectory(testDirectory);
            }

            //create file
            var fileToCreate = Path.Combine(testDirectory, fileName);
            if (File.Exists(fileToCreate))
            {
                File.Delete(fileToCreate);
            }
            return fileToCreate;
        }

        private string GetTestDirectory(string problemName)
        {

            var goodDirectory = GetCurrentDirectory();
            if (!Directory.Exists(goodDirectory))
            {
                return string.Empty;
            }

            var testDirectory = Path.Combine(goodDirectory, problemName);
            if (!Directory.Exists(testDirectory))
            {
                return string.Empty;
            }

            return testDirectory;
        }

        public string GetCurrentDirectory()
        {
            var myDoc = "/usr/local/etc/";
            var goodDirectory = Path.Combine(myDoc, SOURCES);
            return goodDirectory;
        }

        public string BuildNewDirectory(string oldPath, string folderName)
        {
            var newDirectory = Path.Combine(oldPath, folderName);
            if(!Directory.Exists(newDirectory))
            {
                Directory.CreateDirectory(newDirectory);
            }
            return newDirectory;
        }
        public string GetFileFullName(string problemName, string userName)
        {
            var goodDirectory = GetCurrentDirectory();

            StringBuilder sb = new StringBuilder();
            string problemFullName = Builder.BuildProblemName(problemName, userName);
            sb.Append(problemFullName);
            string sourceName = sb.ToString();

            var fileToCreate = Path.Combine(goodDirectory, sourceName);
            return fileToCreate;
        }

        public string ReadExectutionResult()
        {
            string line;
            StringBuilder executionResult = new StringBuilder();
            using (FileStream fs = new FileStream(ExecutionFileResult, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None))
            {
                StreamReader fileReader = new StreamReader(fs, Encoding.UTF8);
                while ((line = fileReader.ReadLine()) != null)
                {
                    executionResult.Append(line);
                    executionResult.Append("---");
                }
            }
            return executionResult.ToString();
        }

        public bool DeleteFile(string fileInput, string fileOk, string problem)
        {
            var testDirectoryForCurrentProblem = GetTestDirectory(problem);
            if(string.IsNullOrEmpty(testDirectoryForCurrentProblem))
            {
                return false;
            }

            string fileInputFullPath = Path.Combine(testDirectoryForCurrentProblem, fileInput);
            string fileOutputFullPath = Path.Combine(testDirectoryForCurrentProblem, fileOk);

            if (File.Exists(fileInputFullPath))
            {
                File.Delete(fileInputFullPath);
            }

            if(File.Exists(fileOutputFullPath))
            {
                File.Delete(fileOutputFullPath);
            }

            return true;
        }
    }
}
