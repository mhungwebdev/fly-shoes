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

        public async Task<bool> SendMail(string content)
        {
            OpenConnect();
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(MailboxAddress.Parse("mhung.haui.webdev@gmail.com"));
            emailMessage.To.Add (MailboxAddress.Parse("vut5441@gmail.com"));
            emailMessage.Cc.Add (MailboxAddress.Parse("mahhugcoder@gmail.com"));
            emailMessage.Subject = "Test mail";
            emailMessage.Body = new TextPart("html") { Text = "<strong style='color:red'>Test email</strong>" };

            var res = await _smtpClient.SendAsync(emailMessage);
            Disconnect();

            return true;
        }
    }
}
