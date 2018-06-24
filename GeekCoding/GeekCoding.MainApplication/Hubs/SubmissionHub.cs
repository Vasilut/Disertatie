using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekCoding.MainApplication.Hubs
{
    public class SubmissionHub:Hub
    {
        public Task SendMessageToCaller(string message, string subbmisionId)
        {
            return Clients.Caller.SendAsync("SubmissionMessage", subbmisionId, message);
        }

        public Task SendScoreMessageToCaller(string message, string submissionId, string score)
        {
            return Clients.Caller.SendAsync("ExecutionMessage", submissionId, message, score);
        }
    }
}
