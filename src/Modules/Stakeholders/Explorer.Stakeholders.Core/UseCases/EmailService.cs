using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Explorer.Tours.API.Dtos;
using System.Security.Principal;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendEmail(AccountRegistrationDto account, string tokenData)
        {
            var smtpServer = _configuration["SmtpSettings:Server"];
            var smtpPort = int.Parse(_configuration["SmtpSettings:Port"]);
            var smtpUsername = _configuration["SmtpSettings:Username"];
            var smtpPassword = _configuration["SmtpSettings:Password"];
            var senderEmail = _configuration["SmtpSettings:SenderEmail"];

            var emailMessage = CreateEmailMessage(account, tokenData);

            using (var client = new SmtpClient())
            {
                client.Connect(smtpServer, smtpPort, useSsl: false);
                client.Authenticate(smtpUsername, smtpPassword);
                client.Send(emailMessage);
                client.Disconnect(true);
            }
        }

        private MimeMessage CreateEmailMessage(AccountRegistrationDto account, string tokenData)
        {
            var message = new MimeMessage();
            var senderName = "Explorer";

            message.From.Add(new MailboxAddress(senderName, _configuration["SmtpSettings:SenderEmail"]));
            message.To.Add(new MailboxAddress(account.Name, account.Email));
            message.Subject = "Verification Email";

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = $"<p>Dear {account.Name},</p>" +
                                   $"<p>Thank you for registering. Please click the following link to verify your email:</p>" +
                                   $"<a href='http://localhost:8080/api/users/verify/{tokenData}'>Verify Email</a>";

            message.Body = bodyBuilder.ToMessageBody();

            return message;
        }

        public void SendPasswordResetEmail(string userName, string userEmail, string secureTokenData)
        {
            var smtpServer = _configuration["SmtpSettings:Server"];
            var smtpPort = int.Parse(_configuration["SmtpSettings:Port"]);
            var smtpUsername = _configuration["SmtpSettings:Username"];
            var smtpPassword = _configuration["SmtpSettings:Password"];
            var senderEmail = _configuration["SmtpSettings:SenderEmail"];

            var emailMessage = CreatePasswordResetEmailMessage(userName, userEmail, secureTokenData);

            using (var client = new SmtpClient())
            {
                client.Connect(smtpServer, smtpPort, useSsl: false);
                client.Authenticate(smtpUsername, smtpPassword);
                client.Send(emailMessage);
                client.Disconnect(true);
            }
        }

        private MimeMessage CreatePasswordResetEmailMessage(string userName, string userEmail, string secureTokenData)
        {
            var message = new MimeMessage();
            var senderName = "Explorer";

            message.From.Add(new MailboxAddress(senderName, _configuration["SmtpSettings:SenderEmail"]));
            message.To.Add(new MailboxAddress(userName, userEmail));
            message.Subject = "Password Reset Email";

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = $"<p>Dear {userName},</p>" +
                                   $"<p>Thank you for being a user of our application.</p>" +
                                   $"<p>Please click the following link to reset your password:</p>" +
                                   $"<a href='http://localhost:4200/reset-password/{secureTokenData}'>Reset password</a>";

            message.Body = bodyBuilder.ToMessageBody();
            
            return message;
        }

		private MimeMessage SendRecommendedToursToEmail(string email, string name, List<long> recommendedToursIds, List<string> tourNames)
		{
			var message = new MimeMessage();
			var senderName = "Explorer";

			message.From.Add(new MailboxAddress(senderName, _configuration["SmtpSettings:SenderEmail"]));
			message.To.Add(new MailboxAddress(name, email));
			message.Subject = "Your recommended tours";

			var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = $"<p>Dear {name},</p>" +
                                  $"<p>Here are your recommended tours:</p>";

			for (int i = 0; i < recommendedToursIds.Count; i++)
			{
				long tourId = recommendedToursIds[i];
				string tourName = tourNames[i];

				bodyBuilder.HtmlBody += $"<p><a href='http://localhost:4200/tour-overview-details/{tourId}'>{tourName}</a></p>";
			}
			message.Body = bodyBuilder.ToMessageBody();

			return message;
		}

		public void SendRecommendedToursEmail(string email, string name, List<long> recommendedToursIds, List<string> tourNames)
		{
			var smtpServer = _configuration["SmtpSettings:Server"];
			var smtpPort = int.Parse(_configuration["SmtpSettings:Port"]);
			var smtpUsername = _configuration["SmtpSettings:Username"];
			var smtpPassword = _configuration["SmtpSettings:Password"];

			var recommendedToursMessage = SendRecommendedToursToEmail(email, name, recommendedToursIds, tourNames);

			using (var client = new SmtpClient())
			{
				client.Connect(smtpServer, smtpPort, useSsl: false);
				client.Authenticate(smtpUsername, smtpPassword);
				client.Send(recommendedToursMessage);
				client.Disconnect(true);
			}
		}

	}
}
