using GeekCoding.MainApplication.Utilities.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekCoding.MainApplication.Utilities
{
    public class SerializeTests : ISerializeTests
    {
        private Dictionary<string, string> _responseDictionary;
        private List<TestModelDto> _listTestModel;
        public SerializeTests()
        {
            _responseDictionary = new Dictionary<string, string>();
            _listTestModel = new List<TestModelDto>();
        }

        public Tuple<string,int> SerializeReponseTest(List<string> tests)
        {
            //each string represent a result "time:1.125---time-wall:1.143---max-rss:4972---csw-voluntary:1---csw-forced:315---cg-mem:2512--- Response: Incorrect!\n"
            //we parse all the result and we'll serialize the list where we store the new results
            int sumPoints = 0;
            foreach (var item in tests)
            {
                string[] execution = item.Split(' ');
                if (execution.Length > 2)
                {
                    string executionResult = execution[0];
                    string executionOutput = execution[2];
                    
                    var options = executionResult.Split(new string[] { "---" }, StringSplitOptions.None);
                    foreach (var metaParameter in options)
                    {
                        var splitOption = metaParameter.Split(':');
                        if (splitOption.Length > 1)
                        {
                            var key = splitOption[0];
                            var val = splitOption[1];

                            _responseDictionary.Add(key, val);
                        }
                    }

                    var testModel = new TestModelDto();
                    if (_responseDictionary.ContainsKey("status"))
                    {
                        //we have an error
                        //see what is the status (TLE, OR MEM)
                        string val = _responseDictionary["status"];
                        UpdateRespondeModel(ref testModel, val);

                    }
                    else
                    {
                        //don't have errors
                        testModel.ExecutionTime = _responseDictionary["time"];
                        testModel.MemoryUsed = _responseDictionary["cg-mem"];

                        if (executionOutput == "Ok!")
                        {
                            testModel.Point = 10;
                            testModel.Message = "Ok!";
                            sumPoints += 10;
                        }
                        else
                        {
                            testModel.Point = 0;
                            testModel.Message = "Incorrect!";
                        }
                    }

                    _listTestModel.Add(testModel);
                }
            }

            var serializedList = JsonConvert.SerializeObject(_listTestModel);
            return new Tuple<string,int>(serializedList,sumPoints);

        }

        private void UpdateRespondeModel(ref TestModelDto testModel, string status)
        {
            testModel.Point = 0;
            switch (status)
            {
                case "TO":
                    {
                        testModel.ExecutionTime = "Time Limit Exceeeded!";
                        testModel.Message = "Time Limit Exceeded!";
                        testModel.MemoryUsed = _responseDictionary["cg-mem"];
                        testModel.Point = 0;
                        break;
                    }
                case "SG":
                    {
                        testModel.ExecutionTime = "Memory limit exceeeded!";
                        testModel.Message = "Caught fatal signal 9!";
                        testModel.MemoryUsed = "Caught fatal signal 9";
                        testModel.Point = 0;
                        break;
                    }
                case "RE":
                    {
                        testModel.ExecutionTime = "Runtime Error!";
                        testModel.Message = "Runtime Error!";
                        testModel.MemoryUsed = "Runtime Error";
                        testModel.Point = 0;
                        break;
                    }
            }
        }

    }
}
