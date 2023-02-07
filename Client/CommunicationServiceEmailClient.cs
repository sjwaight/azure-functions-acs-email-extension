using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Communication.Email;
using Azure.Communication.Email.Models;

namespace SiliconValve.Demo.CommunicationServiceEmail.Client
{
    internal class CommunicationServiceEmailClient : ICommunicationServiceEmailClient
    {
        private EmailClient client;

        public CommunicationServiceEmailClient(string connectionString)
        {
            client = new EmailClient(connectionString);
        }

        public async Task<SendEmailResult> SendMessageAsync(EmailMessage msg, CancellationToken cancellationToken = default(CancellationToken))
        {
            SendEmailResult sendResult = await client.SendAsync(msg, cancellationToken);

            if (sendResult == null || string.IsNullOrEmpty(sendResult.MessageId))
            {
                throw new InvalidOperationException("Azure Communication Service Email Service failed to send message, please retry.");
            }

            return sendResult;
        }
    }
}