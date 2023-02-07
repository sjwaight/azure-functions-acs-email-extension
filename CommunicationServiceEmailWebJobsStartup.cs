using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Hosting;
using SiliconValve.Demo.CommunicationServiceEmail;

[assembly: WebJobsStartup(typeof(SiliconValve.Demo.CommunicationServiceEmail.CommunicationServiceEmailWebJobsStartup))]

namespace SiliconValve.Demo.CommunicationServiceEmail
{
    public class CommunicationServiceEmailWebJobsStartup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.AddExtension<CommunicationServiceEmailConfigProvider>();
        }
    }
}