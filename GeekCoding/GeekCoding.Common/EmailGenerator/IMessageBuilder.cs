using System;
using System.Collections.Generic;
using System.Text;

namespace GeekCoding.Common.EmailGenerator
{
    public interface IMessageBuilder
    {
        EmailBuilder AddSubject(string subject);

        EmailBuilder AddBody(string body);

        EmailBuilder AddReceiver(string receiver);

        void BuildAndSend();
    }
}
