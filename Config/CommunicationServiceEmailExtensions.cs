using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using SiliconValve.Demo.CommunicationServiceEmail.Config;
using Microsoft.Extensions.DependencyInjection;
using SiliconValve.Demo.CommunicationServiceEmail.Bindings;

namespace SiliconValve.Demo.CommunicationServiceEmail
{
    public static class SendGridWebJobsBuilderExtensions
    {
        public static IWebJobsBuilder AddCommunicationServiceEmail(this IWebJobsBuilder builder)
        { 
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.AddExtension<CommunicationServiceEmailConfigProvider>()
                .ConfigureOptions<CommunicationServiceEmailOptions>((rootConfig, extensionPath, options) =>
                {
                    // Set the default, which can be overridden.
                    options.ConnectionString = rootConfig[CommunicationServiceEmailConfigProvider.AzureWebJobsCommunicationServicesEmailConnectionStringName];

                    IConfigurationSection section = rootConfig.GetSection(extensionPath);
                    CommunicationServiceEmailAsyncCollector.ApplyConfiguration(section, options);
                });

            builder.Services.AddSingleton<ICommunicationServiceEmailClientFactory, DefaultCommunicationServiceEmailClientFactory>();

            return builder;
        }

        public static IWebJobsBuilder AddCommunicationServiceEmail(this IWebJobsBuilder builder, Action<CommunicationServiceEmailOptions> configure)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            builder.AddCommunicationServiceEmail();
            builder.Services.Configure(configure);

            return builder;
        }
    }
}