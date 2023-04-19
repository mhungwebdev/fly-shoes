using FlyShoes.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using FirebaseAdmin.Messaging;
using System.Reactive.Subjects;
using FlyShoes.Common.Models;

namespace FlyShoes.DAL.Implements
{
    public class EmailService : IEmailService
    {
        SmtpClient _smtpClient;

        public EmailService()
        {
            
        }

        void OpenConnect()
        {
            _smtpClient = new SmtpClient();
            _smtpClient.Connect("smtp-relay.sendinblue.com", 587, SecureSocketOptions.Auto);
            _smtpClient.Authenticate("mhung.haui.webdev@gmail.com", "B2yJCK08gMYIjdUt");
        }

        void Disconnect()
        {
            _smtpClient.Disconnect(true);
        }

        public async Task<bool> SendMail(FlyEmail fsEmail)
        {
            OpenConnect();
            var emailMessage = new MimeMessage();
            var from = string.IsNullOrEmpty(fsEmail.From) ? "mhung.haui.webdev@gmail.com" : fsEmail.From;

            emailMessage.From.Add(MailboxAddress.Parse(from));
            emailMessage.To.Add (MailboxAddress.Parse(fsEmail.To));
            if(fsEmail.Cc != null && fsEmail.Cc.Count > 0)
            {
                foreach(var cc in fsEmail.Cc)
                {
                    emailMessage.Cc.Add (MailboxAddress.Parse(cc));
                }
            }
            emailMessage.Subject = fsEmail.Subject;
            emailMessage.Body = new TextPart("html") { Text = fsEmail.EmailContent };

            var res = await _smtpClient.SendAsync(emailMessage);
            Disconnect();

            return true;
        }
    }
}
