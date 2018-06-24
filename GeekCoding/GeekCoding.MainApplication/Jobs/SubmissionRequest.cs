using GeekCoding.Compilation.Api.Model;
using GeekCoding.MainApplication.Hubs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GeekCoding.MainApplication.Jobs
{
    public class SubmissionRequest
    {
        private SubmissionHub _submissionHub;

        public SubmissionRequest(SubmissionHub submissionHub)
        {
            _submissionHub = submissionHub;
        }
        public async Task MakeSubmissionRequestAsync(CompilationModel compilationModel, string _compilationApi,
                                                     string userName, string _executionApi,
                                                     string submissionId)
        {
            var client = new HttpClient();
            var serializedData = JsonConvert.SerializeObject(compilationModel);
            var httpContent = new StringContent(serializedData, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(_compilationApi, httpContent);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = await response.Content.ReadAsStringAsync();
                var content = JsonConvert.DeserializeObject<ResponseModel>(result);

                //update with signal r the response for the submission
                await _submissionHub.SendMessageToCaller("Muie Steaua", submissionId);

                if (content.CompilationResponse == "SUCCESS")
                {
                    //call the api to execute... not done yet.. (linux)
                    var executionModel = new ExecutionModel { MemoryLimit = "10000", ProblemName = compilationModel.ProblemName, UserName = userName, TimeLimit = "2" };
                    var serializedExecutionData = JsonConvert.SerializeObject(executionModel);
                    var httpContentExecution = new StringContent(serializedExecutionData, Encoding.UTF8, "application/json");
                    var responseExecution = await client.PostAsync(_executionApi, httpContent);
                    if (responseExecution.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var resultEx = await responseExecution.Content.ReadAsStringAsync();
                        //another signal r notification
                        await _submissionHub.SendScoreMessageToCaller("Executat", submissionId, "70");

                        var x = 2;
                    }

                }
            }
        }
    }
}
