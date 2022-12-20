using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid.Helpers.Mail;
using SendGrid;
using MailKit.Security;
using MimeKit;
using MailKit.Net.Smtp;

namespace THUD_TN408.Authorization
{
	public class EmailSender : IEmailSender
	{
		private readonly ILogger _logger;
		public MailSettings MailSettings { get; }

		public EmailSender(IOptions<MailSettings> mailSettings, ILogger<EmailSender> logger)
		{
			MailSettings = mailSettings.Value;
			_logger = logger;
		}

		

		//public async Task SendEmailAsync(string toEmail, string subject, string message)
		//{
		//	if (string.IsNullOrEmpty(Options.SendGridKey))
		//	{
		//		throw new Exception("Null SendGridKey");
		//	}
		//	await Execute(Options.SendGridKey, subject, message, toEmail);
		//}

		//public async Task Execute(string apiKey, string subject, string message, string toEmail)
		//{
		//	var client = new SendGridClient(apiKey);
		//	var msg = new SendGridMessage()
		//	{
		//		From = new EmailAddress("anhb1910185@student.ctu.edu.vn", "Password Recovery"),
		//		Subject = subject,
		//		PlainTextContent = message,
		//		HtmlContent = message
		//	};
		//	msg.AddTo(new EmailAddress(toEmail));

		//	// Disable click tracking.
		//	// See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
		//	msg.SetClickTracking(false, false);
		//	var response = await client.SendEmailAsync(msg);
		//	_logger.LogInformation(response.IsSuccessStatusCode
		//						   ? $"Email to {toEmail} queued successfully!"
		//						   : $"Failure Email to {toEmail}");
		//}
		public async Task SendEmailAsync(string email, string subject, string htmlMessage)
		{
			var myEmail = new MimeMessage();
			myEmail.Sender = MailboxAddress.Parse(MailSettings.Mail);
			myEmail.To.Add(MailboxAddress.Parse(email));
			myEmail.Subject = subject;
			var builder = new BodyBuilder();
			
			builder.HtmlBody = htmlMessage;
			myEmail.Body = builder.ToMessageBody();
			using var smtp = new SmtpClient();
			smtp.Connect(MailSettings.Host, MailSettings.Port, SecureSocketOptions.StartTls);
			smtp.Authenticate(MailSettings.Mail, MailSettings.Password);
			await smtp.SendAsync(myEmail);
			smtp.Disconnect(true);
		}
	}
}
