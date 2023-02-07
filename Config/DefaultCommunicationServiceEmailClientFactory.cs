using SiliconValve.Demo.CommunicationServiceEmail.Client;

namespace SiliconValve.Demo.CommunicationServiceEmail.Config
{
    internal class DefaultCommunicationServiceEmailClientFactory : ICommunicationServiceEmailClientFactory
    {
        public ICommunicationServiceEmailClient Create(string connectionString)
        {
            return new CommunicationServiceEmailClient(connectionString);
        }
    }
}