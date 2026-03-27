using QueueManagement.Domain.Entities;
using QueueManagement.Domain.Interfaces;
using QueueManagement.Infrastructure.Persistence.Context;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Runtime;
using QueueManagement.Domain.Entities.DTOs;
using Microsoft.Extensions.Options;

namespace QueueManagement.Infrastructure.Persistence.Repositories
{
    public class EmailLogRepository : GenericRepository<EmailLog>, IEmailLogRepository
    {
        private readonly EmailSettings _settings;
        public EmailLogRepository(QueueManagementDbContext context, IOptions<EmailSettings> settings) : base(context)
        {
            _settings = settings.Value;
        }

        public async Task SendAsync(string toEmail, string subject, string body)
        {

    
            var smtpClient = new SmtpClient(_settings.SmtpServer)
            {
                Port = _settings.Port,
                Credentials = new NetworkCredential(
                    _settings.SenderEmail,
                    _settings.AppPassword
                ),
                EnableSsl = true,
            };

            var mail = new MailMessage
            {
                From = new MailAddress(_settings.SenderEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mail.To.Add(toEmail);

            await smtpClient.SendMailAsync(mail);
        }
    }
}

