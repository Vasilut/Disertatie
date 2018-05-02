using System;
using System.Collections.Generic;
using System.Text;

namespace GeekCoding.Common.EmailGenerator
{
    public class Email : IMessage
    {
        public string To { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }
    }
}
