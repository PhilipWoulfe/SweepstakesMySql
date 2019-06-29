using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Google.Api.Gax.Rest;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using RestSharp;
using RestSharp.Authenticators;

namespace SweepstakesAppEngineMySQL.Services
{
    public class EmailSender : IEmailSender
    {
        public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }

        public AuthMessageSenderOptions Options { get; } //set only via Secret Manager

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Execute(Options.MailgunKey, Options.BaseURL, Options.DomainName, subject, message, email);
        }

        public Task Execute(string apiKey, string url, string domain, string subject, string message, string email)
        {
            var client = new RestClient
            {
                BaseUrl = new Uri(url + "/messages"),
                Authenticator =
                new HttpBasicAuthenticator("api", apiKey)
            };

            //client.BaseUrl = new Uri("https://api.mailgun.net/v3");
            //client.Authenticator =
            //    new HttpBasicAuthenticator("api", apiKey);

            var request = new RestRequest();
            request.AddParameter("domain", domain, ParameterType.UrlSegment);
            //request.Resource = "{domain}/messages";
            request.AddParameter("from", "Excited User <noreply@" + domain + ">");
            request.AddParameter("to", email);
            request.AddParameter("subject", subject);
            request.AddParameter("text", message);
            request.Method = Method.POST;
            return client.ExecuteTaskAsync(request);

            //var msg = new SendGridMessage()
            //{
            //    From = new EmailAddress("Joe@contoso.com", "Joe Smith"),
            //    Subject = subject,
            //    PlainTextContent = message,
            //    HtmlContent = message
            //};
            //msg.AddTo(new EmailAddress(email));

            //// Disable click tracking.
            //// See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            //msg.SetClickTracking(false, false);

            //return client.SendEmailAsync(msg);
        }
    }
}
