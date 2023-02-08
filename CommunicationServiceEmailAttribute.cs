using System;
using Microsoft.Azure.WebJobs.Description;

namespace SiliconValve.Demo.CommunicationServiceEmail
{
    [Binding]
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    public class CommunicationServiceEmailAttribute : Attribute
    {
        [AppSetting]
        public string ConnectionString {get; set;}

        [AutoResolve]
        public string SenderAddress {get; set;}

        [AutoResolve]
        public string SubjectLine {get; set;}

        [AutoResolve]
        public string RecipientAddress {get; set;}

        [AutoResolve]
        public string PlainTextEmailBody {get; set;}

        [AutoResolve]
        public string HtmlEmailBody {get; set;}
    }
}