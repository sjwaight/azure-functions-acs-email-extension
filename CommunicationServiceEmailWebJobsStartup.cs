using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;

[assembly: WebJobsStartup(typeof(SiliconValve.Demo.CommunicationServiceEmail.CommunicationServiceEmailWebJobsStartup))]

namespace SiliconValve.Demo.CommunicationServiceEmail
{
    public class CommunicationServiceEmailWebJobsStartup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.AddCommunicationServiceEmail();
        }
    }
}