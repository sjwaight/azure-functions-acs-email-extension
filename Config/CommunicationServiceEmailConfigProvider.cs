using System;
using System.Collections.Concurrent;
using System.Linq;
using Azure.Communication.Email.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Extensions.Options;
using SiliconValve.Demo.CommunicationServiceEmail.Bindings;
using SiliconValve.Demo.CommunicationServiceEmail.Client;
using SiliconValve.Demo.CommunicationServiceEmail.Config;

namespace SiliconValve.Demo.CommunicationServiceEmail
{
    [Extension("CommunicationServiceEmail")]
    internal class CommunicationServiceEmailConfigProvider : IExtensionConfigProvider
    {
        internal const string AzureWebJobsCommunicationServicesEmailConnectionStringName = "AzureWebJobsAcsEmailConnectionString";

        private readonly IOptions<CommunicationServiceEmailOptions> configOptions;
        private ConcurrentDictionary<string, ICommunicationServiceEmailClient> emailClientCache = new ConcurrentDictionary<string, ICommunicationServiceEmailClient>();

        public CommunicationServiceEmailConfigProvider(IOptions<CommunicationServiceEmailOptions> options, ICommunicationServiceEmailClientFactory clientFactory)
        {
            configOptions = options;
            ClientFactory = clientFactory;
        }

        internal ICommunicationServiceEmailClientFactory ClientFactory { get; set; }

        public void Initialize(ExtensionConfigContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var rule = context.AddBindingRule<CommunicationServiceEmailAttribute>();
            rule.AddValidator(ValidateBinding);
            rule.BindToCollector<EmailMessage>(CreateCollector);
      
        }
 
        private IAsyncCollector<EmailMessage> CreateCollector(CommunicationServiceEmailAttribute attribute)
        {            
            string connectionString = FirstOrDefault(attribute.ConnectionString, configOptions.Value.ConnectionString);
            ICommunicationServiceEmailClient emailClient = emailClientCache.GetOrAdd(connectionString, a => ClientFactory.Create(a));
            return new CommunicationServiceEmailAsyncCollector(configOptions.Value, attribute, emailClient);
        }

        private static string FirstOrDefault(params string[] values)
        {
            return values.FirstOrDefault(v => !string.IsNullOrEmpty(v));
        }       

        
        private void ValidateBinding(CommunicationServiceEmailAttribute attribute, Type type)
        {
            ValidateBinding(attribute);
        }

        private void ValidateBinding(CommunicationServiceEmailAttribute attribute)
        {
            string connectionString = FirstOrDefault(attribute.ConnectionString, configOptions.Value.ConnectionString);

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(
                    $"The Communication Service Connection String must be set either via an '{AzureWebJobsCommunicationServicesEmailConnectionStringName}' app setting, via an '{AzureWebJobsCommunicationServicesEmailConnectionStringName}' environment variable, or directly in code via {nameof(CommunicationServiceEmailOptions)}.{nameof(CommunicationServiceEmailOptions.ConnectionString)} or {nameof(CommunicationServiceEmailAttribute)}.{nameof(CommunicationServiceEmailAttribute.ConnectionString)}.");
            }
        } 
    }
}