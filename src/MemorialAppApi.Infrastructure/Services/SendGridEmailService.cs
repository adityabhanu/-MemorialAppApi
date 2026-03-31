using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemorialAppApi.Infrastructure.Services
{
    public class SendGridEmailService : IEmailService
    {
        private readonly string _apiKey;

        public SendGridEmailService(IConfiguration config)
        {
            _apiKey = config["SendGrid:ApiKey"];
        }

        public async Task SendAsync(string to, string subject, string body)
        {
            var client = new SendGridClient(_apiKey);

            var msg = MailHelper.CreateSingleEmail(
                new EmailAddress("adityabhanu.pandey@gmail.com"),
                new EmailAddress(to),
                subject,
                body,
                body
            );

            var response = await client.SendEmailAsync(msg);

            var responseBody = await response.Body.ReadAsStringAsync();

            Console.WriteLine($"SendGrid Status: {response.StatusCode}");
            Console.WriteLine($"SendGrid Response: {responseBody}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"SendGrid failed: {responseBody}");
            }
        }
    }
}
