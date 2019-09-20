﻿using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Threading.Tasks;

namespace KNI_D6_web.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfiguration config;

        public EmailService(IOptions<EmailConfiguration> config)
        {
            this.config = config.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress(config.AdministrationEmailName, config.AdministrationEmail));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = htmlMessage
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(config.HostEmail, config.HostPort);

                client.AuthenticationMechanisms.Remove("XOAUTH2");

                await client.AuthenticateAsync(config.AdministrationEmail, config.AdministrationPassword);
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
        }
    }
}
