using Microsoft.Azure.WebJobs;
using Azure.Communication.Email;
using Azure.Communication.Email.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Configuration;
using SiliconValve.Demo.CommunicationServiceEmail.Config;
using Microsoft.Extensions.DependencyInjection;

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
                    // SendGridHelpers.ApplyConfiguration(section, options);
                });

            builder.Services.AddSingleton<ICommunicationServiceEmailClientFactory, DefaultCommunicationServiceEmailClientFactory>();

            return builder;
        }



    }
}