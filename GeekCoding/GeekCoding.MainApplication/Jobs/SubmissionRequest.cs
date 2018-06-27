using GeekCoding.Compilation.Api.Model;
using GeekCoding.Data.Models;
using GeekCoding.MainApplication.Hubs;
using GeekCoding.MainApplication.ResponseModelDTO;
using GeekCoding.MainApplication.Utilities;
using GeekCoding.Repository.Interfaces;
using Microsoft.AspNetCore.SignalR;
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
        private ISubmisionRepository _submissionRepository;
        private IHubContext<SubmissionHub> _hubContext;
        private ISerializeTests _serializeTests;

        public SubmissionRequest(SubmissionHub submissionHub, ISubmisionRepository submissionRepository,
                                 IHubContext<SubmissionHub> hubContext, ISerializeTests serializeTests)
        {
            _submissionHub = submissionHub;
            _submissionRepository = submissionRepository;
            _hubContext = hubContext;
            _serializeTests = serializeTests;
           
        }
        public async Task MakeSubmissionRequestAsync(SubmisionDto submision, string _compilationApi,
                                                     string _executionApi)
        {
            //update the status of the submission
            UpdateSubmissionStatus(submision.SubmissionId, SubmissionStatus.Compiling,string.Empty);
            //notify signal r to compiling status
            //await NotifyResponse(MessageType.CompilationMessage, SubmissionStatus.Compiling.ToString(), submision.SubmissionId.ToString(), "0");

            var compilationModel = new CompilationModel { Content = submision.Content, Language = submision.Compilator,
                                                          ProblemName = submision.ProblemName, Username = submision.UserName };
            var client = new HttpClient();
            var serializedData = JsonConvert.SerializeObject(compilationModel);
            var httpContent = new StringContent(serializedData, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(_compilationApi, httpContent);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = await response.Content.ReadAsStringAsync();
                var content = JsonConvert.DeserializeObject<ResponseCompilationModel>(result);

                //update with signal r the response for the submission
                //await NotifyResponse(MessageType.CompilationMessage, SubmissionStatus.Compiled.ToString(), submision.SubmissionId.ToString(), "0");
                var task =  _hubContext.Clients.All.SendAsync("SubmissionMessage", "Salut Dinamo", submision.SubmissionId.ToString());
                if(task != null)
                {
                    await task;
                }

                if (content.CompilationResponse == "SUCCESS")
                {
                    //update the status of the submission
                    UpdateSubmissionStatus(submision.SubmissionId, SubmissionStatus.Compiled,content.CompilationResponse);

                    //notify with signal r
                    //await NotifyResponse(MessageType.CompilationMessage, SubmissionStatus.Compiled.ToString(), submision.SubmissionId.ToString(), "0");
                    
                    //call the api to execute
                    await ExecuteSubmission(submision, _executionApi);

                }
                else
                {
                    //update status not compiled
                    UpdateSubmissionStatus(submision.SubmissionId, SubmissionStatus.CompilationError,content.OutputMessage);

                    //notify with signal r
                    //await NotifyResponse(MessageType.CompilationMessage, SubmissionStatus.CompilationError.ToString(), submision.SubmissionId.ToString(), "0");
                }
            }
        }

        private async Task ExecuteSubmission(SubmisionDto submision, string _executionApi)
        {
            var client = new HttpClient();

            var executionModel = new ExecutionModel { MemoryLimit = submision.MemoryLimit, ProblemName = submision.ProblemName,
                                                      UserName = submision.UserName, TimeLimit = submision.TimeLimit};
            var serializedExecutionData = JsonConvert.SerializeObject(executionModel);
            var httpContentExecution = new StringContent(serializedExecutionData, Encoding.UTF8, "application/json");
            var responseExecution = await client.PostAsync(_executionApi, httpContentExecution);
            if (responseExecution.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var resultExecution = await responseExecution.Content.ReadAsStringAsync();
                List<string> tests = new List<string>();
                tests.Add(resultExecution);
                var serializedData = _serializeTests.SerializeReponseTest(tests);
                //save in db the serializedData

                //another signal r notification
                var taskExecution = _hubContext.Clients.All.SendAsync("ExecutionMessage", "Executat", submision.SubmissionId.ToString(), serializedData.Item2.ToString());
                if (taskExecution != null)
                {
                    await taskExecution;
                }
                //notify with signalR
                //await NotifyResponse(MessageType.ExecutionMessage, SubmissionStatus.Executed.ToString(), submision.SubmissionId.ToString(), "70");

                var x = 2;
            }
        }

        private void UpdateSubmissionStatus(Guid submissionId, SubmissionStatus submissionStatus, string messageOfCompilation)
        {
            var submissionToUpdate = _submissionRepository.GetItem(submissionId);
            if (submissionToUpdate != null)
            {
                submissionToUpdate.StateOfSubmision = submissionStatus.ToString();
                submissionToUpdate.MessageOfSubmision = messageOfCompilation;
                _submissionRepository.Update(submissionToUpdate);
                _submissionRepository.Save();

            }
        }

        private async Task NotifyResponse(MessageType messageType, string message, string submissionId, string score)
        {
            if(messageType == MessageType.CompilationMessage)
            {
                var task = _hubContext.Clients.All.SendAsync("SubmissionMessage", message, submissionId);
                if(task != null)
                {
                    await task;
                }
            }
            else
            {
                var taskExecution = _hubContext.Clients.All.SendAsync("ExecutionMessage", message, submissionId, score);
                if(taskExecution != null)
                {
                    await taskExecution;
                }
            }
        }
        
    }
}
