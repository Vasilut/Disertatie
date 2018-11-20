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
using System.Threading;
using System.Threading.Tasks;

namespace GeekCoding.MainApplication.Jobs
{
    public class SubmissionRequest
    {
        private SubmissionHub _submissionHub;
        private ISubmisionRepository _submissionRepository;
        private IEvaluationRepository _evaluationRepository;
        private IHubContext<SubmissionHub> _hubContext;
        private ISerializeTests _serializeTests;
        static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);


        public SubmissionRequest(SubmissionHub submissionHub, ISubmisionRepository submissionRepository,
                                 IHubContext<SubmissionHub> hubContext, ISerializeTests serializeTests,
                                 IEvaluationRepository evaluationRepository)
        {
            _submissionHub = submissionHub;
            _submissionRepository = submissionRepository;
            _evaluationRepository = evaluationRepository;
            _hubContext = hubContext;
            _serializeTests = serializeTests;

        }
        public async Task MakeSubmissionRequestAsync(SubmisionDto submision, string _compilationApi,
                                                     string _executionApi)
        {
            UpdateSubmissionStatus(submision.SubmissionId, SubmissionStatus.Compiling, string.Empty, 0);
            //notify signal r to compiling status
            await NotifyResponse(MessageType.CompilationMessage, SubmissionStatus.Compiling.ToString(), submision.SubmissionId.ToString(), "0");

            var compilationModel = new CompilationModel
            {
                Content = submision.Content,
                Language = submision.Compilator,
                ProblemName = submision.ProblemName,
                Username = submision.UserName
            };
            var client = new HttpClient();
            var serializedData = JsonConvert.SerializeObject(compilationModel);
            var httpContent = new StringContent(serializedData, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(_compilationApi, httpContent);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = await response.Content.ReadAsStringAsync();
                var content = JsonConvert.DeserializeObject<ResponseCompilationModel>(result);

                if (content.CompilationResponse == "SUCCESS")
                {
                    UpdateSubmissionStatus(submision.SubmissionId, SubmissionStatus.Compiled, content.OutputMessage, 0);

                    //notify with signal r
                    await NotifyResponse(MessageType.CompilationMessage, SubmissionStatus.Compiled.ToString(), submision.SubmissionId.ToString(), "0");

                    //call the api to execute
                    await semaphoreSlim.WaitAsync();
                    try
                    {
                        await ExecuteSubmission(submision, _executionApi);
                    }
                    finally
                    {
                        semaphoreSlim.Release();
                    }
                }
                else
                {
                    UpdateSubmissionStatus(submision.SubmissionId, SubmissionStatus.CompilationError, content.OutputMessage, 0);

                    //notify with signal r
                    await NotifyResponse(MessageType.CompilationMessage, SubmissionStatus.CompilationError.ToString(), submision.SubmissionId.ToString(), "0");
                }
            }
            else
            {
                //server error
                UpdateSubmissionStatus(submision.SubmissionId, SubmissionStatus.ServerError, response.ReasonPhrase, 0);
            }
        }

        private async Task ExecuteSubmission(SubmisionDto submision, string _executionApi)
        {
            var client = new HttpClient();

            var executionModel = new ExecutionModel
            {
                MemoryLimit = submision.MemoryLimit,
                ProblemName = submision.ProblemName,
                UserName = submision.UserName,
                TimeLimit = submision.TimeLimit,
                Compilator = submision.Compilator,
                FileName = submision.FileName,
                NumberOfTests = submision.NumberOfTests
            };
            var serializedExecutionData = JsonConvert.SerializeObject(executionModel);
            var httpContentExecution = new StringContent(serializedExecutionData, Encoding.UTF8, "application/json");
            var responseExecution = await client.PostAsync(_executionApi, httpContentExecution);

            if (responseExecution.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var resultExecution = await responseExecution.Content.ReadAsStringAsync();
                var responseExecutionModels = DeserializeExecutionResponse(resultExecution);
                var serializedData = _serializeTests.SerializeReponseTest(responseExecutionModels);
                //save in db the serializedData
                var evaluationModel = new Evaluation
                {
                    EvaluationId = Guid.NewGuid(),
                    EvaluationResult = serializedData.Item1,
                    Score = serializedData.Item2,
                    SubmisionId = submision.SubmissionId
                };
                await _evaluationRepository.AddAsync(evaluationModel);

                UpdateSubmissionStatus(submision.SubmissionId, SubmissionStatus.Executed, string.Empty, serializedData.Item2);
                //notify with signalR
                await NotifyResponse(MessageType.ExecutionMessage, SubmissionStatus.Executed.ToString(), submision.SubmissionId.ToString(), serializedData.Item2.ToString());

                var x = 2;
            }
            else
            {
                //server error
                UpdateSubmissionStatus(submision.SubmissionId, SubmissionStatus.ServerError, responseExecution.ReasonPhrase, 0);
            }
        }

        private List<ResponseExecutionModel> DeserializeExecutionResponse(string execututionResponse)
        {
            List<string> result = new List<string>();
            var listDeserialized = JsonConvert.DeserializeObject<List<ResponseExecutionModel>>(execututionResponse);
            return listDeserialized;
        }

        private void UpdateSubmissionStatus(Guid submissionId, SubmissionStatus submissionStatus, string messageOfCompilation, int score)
        {
            var submissionToUpdate = _submissionRepository.GetItem(submissionId);
            if (submissionToUpdate != null)
            {
                submissionToUpdate.StateOfSubmision = submissionStatus.ToString();
                submissionToUpdate.Score = score;
                if (messageOfCompilation != string.Empty)
                {
                    submissionToUpdate.MessageOfSubmision = messageOfCompilation;
                }
                _submissionRepository.Update(submissionToUpdate);
                _submissionRepository.Save();

            }
        }

        private async Task NotifyResponse(MessageType messageType, string message, string submissionId, string score)
        {
            if (messageType == MessageType.CompilationMessage)
            {
                var task = _hubContext.Clients.All.SendAsync("SubmissionMessage", message, submissionId);
                if (task != null)
                {
                    await task;
                }
            }
            else
            {
                var taskExecution = _hubContext.Clients.All.SendAsync("ExecutionMessage", message, submissionId, score);
                if (taskExecution != null)
                {
                    await taskExecution;
                }
            }
        }

    }
}
