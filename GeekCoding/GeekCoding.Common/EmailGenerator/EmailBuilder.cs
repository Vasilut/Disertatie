using System;
using System.Collections.Generic;
using System.Text;

namespace GeekCoding.Common.EmailGenerator
{
    public class EmailBuilder : IMessageBuilder
    {
        private Email _email = new Email();
        public EmailBuilder AddBody(string body)
        {
            _email.Body = body;
            return this;
        }

        public EmailBuilder AddReceiver(string receiver)
        {
            _email.To = receiver;
            return this;
        }

        public EmailBuilder AddSubject(string subject)
        {
            _email.Subject = subject;
            return this;
        }

        public void BuildAndSend()
        {
            if (_email.To != null && _email.Subject != null && _email.Body != null)
            {
                EmailSender.SendMail(_email);
            }
        }
    }
}
