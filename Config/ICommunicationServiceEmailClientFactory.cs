using SiliconValve.Demo.CommunicationServiceEmail.Client;

namespace SiliconValve.Demo.CommunicationServiceEmail.Config
{
    internal interface ICommunicationServiceEmailClientFactory
    {
        ICommunicationServiceEmailClient Create(string connectionString);
    }
}