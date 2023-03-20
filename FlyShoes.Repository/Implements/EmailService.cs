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
            _smtpClient.Connect("smtp-relay.sendinblue.com", 587, SecureSocketOptions.SslOnConnect);
            _smtpClient.Authenticate("fly-shoes", "xsmtpsib-5bec5504846f7fd1f1c6f83c7f575835281737dfa9c41372c3d435a7fb20b2b9-t8Pm1FV3NH7WhQ5d");
        }

        void Disconnect()
        {
            _smtpClient.Disconnect(true);
        }

        public async Task<bool> SendMail(string content)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(MailboxAddress.Parse("mhung.haui.webdev@gmail.com"));
            emailMessage.To.Add (MailboxAddress.Parse("mahhugcoder@gmail.com"));
            emailMessage.Subject = "Test mail";
            emailMessage.Body = new TextPart("html") { Text = content };

            await _smtpClient.SendAsync(emailMessage);

            return true;
        }
    }
}
