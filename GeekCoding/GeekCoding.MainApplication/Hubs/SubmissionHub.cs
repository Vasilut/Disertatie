using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekCoding.MainApplication.Hubs
{
    public class SubmissionHub : Hub
    {
        public Task SendMessageToCaller(string message, string subbmisionId)
        {
            if (Clients != null && Clients.Caller != null)
            {
                return Clients.Caller.SendAsync("SubmissionMessage", subbmisionId, message);
            }

            return null;

        }

        public Task SendScoreMessageToCaller(string message, string submissionId, string score)
        {
            if (Clients != null && Clients.Caller != null)
            {
                return Clients.Caller.SendAsync("ExecutionMessage", submissionId, message, score);
            }
            return null;
        }

        public Task SendSubmittedMessageToCaller(string message)
        {
            if (Clients != null && Clients.Caller != null)
            {
                return Clients.Caller.SendAsync("ProblemSubmitted", message);
            }
            return null;
        }
    }
}
