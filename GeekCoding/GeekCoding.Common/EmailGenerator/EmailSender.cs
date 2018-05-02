using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace GeekCoding.Common.EmailGenerator
{
    public class EmailSender
    {
        public static void SendMail(IMessage email)
        {
            try
            {
                Email mail = (Email)email;
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                SmtpServer.Port = 587;
                SmtpServer.Credentials = new NetworkCredential("geekcodingoperator@gmail.com", "");
                SmtpServer.EnableSsl = true;
                MailMessage mailM = new MailMessage("geekcodingoperator@gmail.com", mail.To, mail.Subject, mail.Body);
                SmtpServer.Send(mailM);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
                throw;
            }
        }
    }
}
