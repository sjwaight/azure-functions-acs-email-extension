using Microsoft.Azure.WebJobs;
using Azure.Communication.Email;
using Azure.Communication.Email.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Collections.Concurrent;
using SiliconValve.Demo.CommunicationServiceEmail.Client;

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

            if(mailMessage.Content == null || string.IsNullOrEmpty(mailMessage.Content.Html) || string.IsNullOrEmpty(mailMessage.Content.PlainText))
            {
                throw new ArgumentException("Missing message body. You must supply a message body via either the HtmlEmailBody or PlainTextEmailBody properties.");
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
            EmailAddress recipientEmail = null;
            string subjectLine = "";

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
                    recipientAddress = to;
                }
                else if (!string.IsNullOrEmpty(options.RecipientAddress))
                {
                    recipientAddress = new EmailAddress(options.RecipientAddress);
                }
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
            }

            if(senderEmail == null && recipientAddress == null && string.IsNullOrEmpty(subjectLine)
        }


        internal static EmailAddress Apply(EmailAddress current, string value)
        {
            EmailAddress mail;
            if (TryParseAddress(value, out mail))
            {
                return mail;
            }
            return current;
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
                // MailAddress will auto-parse the name from a string like "testuser@test.com <Test User>"
                EmailAddress mailAddress = new EmailAddress(value);
                string displayName = string.IsNullOrEmpty(mailAddress.DisplayName) ? null : mailAddress.DisplayName;
                email = new EmailAddress(mailAddress.Email, displayName);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        internal static EmailMessage CreateMessage(string input)
        {
            JObject json = JObject.Parse(input);
            return CreateMessage(json);
        }

        internal static EmailMessage CreateMessage(JObject input)
        {
            return input.ToObject<SendGridMessage>();
        }

        internal static string CreateString(EmailMessage input)
        {
            return CreateString(JObject.FromObject(input));
        }

        internal static string CreateString(JObject input)
        {
            return input.ToString(Formatting.None);
        }

        internal static bool IsRecipientValid(EmailMessage item)
        {
            return item.Personalizations != null &&
                item.Personalizations.Count > 0 &&
                item.Personalizations.All(p => p.Tos != null && !p.Tos.Any(t => string.IsNullOrEmpty(t.Email)));
        }

        internal static void ApplyConfiguration(IConfiguration config, CommunicationServiceEmailOptions options)
        {
            if (config == null)
            {
                return;
            }

            config.Bind(options);

            string to = config.GetValue<string>("to");
            string from = config.GetValue<string>("from");
            options.RecipientAddress = Apply(options.RecipientAddress, to);
            options.SenderAddress = Apply(options.SenderAddress, from);
        }
    }
}