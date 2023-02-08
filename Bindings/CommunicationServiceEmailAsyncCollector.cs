using Microsoft.Azure.WebJobs;
using Azure.Communication.Email.Models;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Collections.Concurrent;
using SiliconValve.Demo.CommunicationServiceEmail.Client;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace SiliconValve.Demo.CommunicationServiceEmail.Bindings
{
    internal class CommunicationServiceEmailAsyncCollector : IAsyncCollector<EmailMessage>
    {
        private CommunicationServiceEmailOptions configOptions;
        private CommunicationServiceEmailAttribute acsEmailAttribute;
        private readonly ConcurrentQueue<EmailMessage> messages = new ConcurrentQueue<EmailMessage>();
        private readonly ICommunicationServiceEmailClient emailServiceClient;

        public CommunicationServiceEmailAsyncCollector(CommunicationServiceEmailOptions options, CommunicationServiceEmailAttribute attribute, ICommunicationServiceEmailClient emailClient)
        {
            acsEmailAttribute = attribute;
            configOptions = options;
            emailServiceClient = emailClient;
        }

        public Task AddAsync(EmailMessage mailMessage, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (mailMessage == null)
            {
                throw new ArgumentNullException(nameof(mailMessage));
            }

            if(mailMessage.Recipients.To.Count == 0)
            {
                throw new ArgumentException("No recipient addresses supplied. You must supply at least one recipient email address.");
            }

            if(string.IsNullOrEmpty(mailMessage.Sender))
            {
                throw new ArgumentException("Missing sender email address. You must supply the configured sender email address for your selected Azure Communication Service Email Service.");
            }

            if(mailMessage.Content == null || (string.IsNullOrEmpty(mailMessage.Content.Html) && string.IsNullOrEmpty(mailMessage.Content.PlainText)))
            {
                throw new ArgumentException("Missing message body. You must supply a message body via either the Html or PlainText Content properties.");
            }

            PopulateDefaultMessageProperties(mailMessage, configOptions, acsEmailAttribute);

            messages.Enqueue(mailMessage);

            return Task.CompletedTask;
        }
 
        public async Task FlushAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            while(messages.TryDequeue(out EmailMessage message))
            {
                await emailServiceClient.SendMessageAsync(message);
            }
        }

         internal static EmailMessage PopulateDefaultMessageProperties(EmailMessage mail, CommunicationServiceEmailOptions options, CommunicationServiceEmailAttribute attribute)
        {
            EmailAddress senderEmail = null;
            EmailRecipients recipients = null;
            string subjectLine = "";
            bool newMessageRequired = false;

            // Apply message defaulting
            if (string.IsNullOrEmpty(mail.Sender))
            {
                if (!string.IsNullOrEmpty(attribute.SenderAddress))
                {
                    EmailAddress from = null;
                    if (!TryParseAddress(attribute.SenderAddress, out from))
                    {
                        throw new ArgumentException("Invalid 'From' address specified");
                    }
                    senderEmail = from;
                }
                else if (!string.IsNullOrEmpty(options.SenderAddress))
                {
                    senderEmail = new EmailAddress(options.SenderAddress);
                }
                newMessageRequired = true;
            }
            else
            {
                senderEmail = new EmailAddress(mail.Sender);
            }

            if (!IsRecipientValid(mail))
            {
                if (!string.IsNullOrEmpty(attribute.RecipientAddress))
                {
                    EmailAddress to = null;
                    if (!TryParseAddress(attribute.RecipientAddress, out to))
                    {
                        throw new ArgumentException("Invalid 'To' address specified");
                    }
                    recipients.To.Add(to);
                }
                else if (!string.IsNullOrEmpty(options.RecipientAddress))
                {
                    recipients.To.Add(new EmailAddress(options.RecipientAddress));
                }
                newMessageRequired = true;
            }
            else
            {
                recipients = mail.Recipients;
            }

            if (string.IsNullOrEmpty(mail.Content.Subject))
            {
                if(!string.IsNullOrEmpty(attribute.SubjectLine))
                {
                    subjectLine = attribute.SubjectLine;
                }
                else if (!string.IsNullOrEmpty(options.Subject))
                {
                    subjectLine = options.Subject;
                }
                newMessageRequired = true;
            }
            else
            {
                subjectLine = mail.Content.Subject;
            }

            if(newMessageRequired)
            {
                EmailContent newContent = new EmailContent(subjectLine)
                                                {   
                                                    Html = mail.Content.Html,
                                                    PlainText = mail.Content.PlainText
                                                };
                return new EmailMessage(senderEmail.Email, newContent, recipients);
            }
            return mail;
        }

        internal static bool IsRecipientValid(EmailMessage item)
        {
            return item.Recipients != null &&
                item.Recipients.To.Count > 0 &&
                item.Recipients.To.All(t => !string.IsNullOrEmpty(t.Email));
        }

        internal static void ApplyConfiguration(IConfiguration config, CommunicationServiceEmailOptions options)
        {
            if (config == null)
            {
                return;
            }

            config.Bind(options);

            options.RecipientAddress = config.GetValue<string>("RecipientAddress");
            options.SenderAddress = config.GetValue<string>("SenderAddress");
            options.Subject = config.GetValue<string>("Subject");
            // these are read, but are not currently used in the logic.
            options.HtmlEmailBody = config.GetValue<string>("HtmlEmailBody");
            options.PlainTextEmailBody = config.GetValue<string>("PlainTextEmailBody");
        }
        
        internal static bool TryParseAddress(string value, out EmailAddress email)
        {
            email = null;

            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            try
            {
                email = new EmailAddress(value);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}