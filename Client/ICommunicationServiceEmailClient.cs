using System.Threading;
using System.Threading.Tasks;
using Azure.Communication.Email.Models;

namespace SiliconValve.Demo.CommunicationServiceEmail.Client
{
    internal interface ICommunicationServiceEmailClient
    {
        Task<SendEmailResult> SendMessageAsync(EmailMessage msg, CancellationToken cancellationToken = default(CancellationToken));
    }
}