using System;
using Microsoft.Azure.WebJobs.Description;

namespace SiliconValve.Demo.CommunicationServiceEmail
{
    [Binding]
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    public class CommunicationServiceEmailAttribute : Attribute
    {
        public CommunicationServiceEmailAttribute()
        {
        }

        public CommunicationServiceEmailAttribute(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public CommunicationServiceEmailAttribute(string connectionString, string senderAddress)
        {
            ConnectionString = connectionString;
            SenderAddress = senderAddress;
        }

        public CommunicationServiceEmailAttribute(string connectionString, string senderAddress, string subjectLine)
        {
            ConnectionString = connectionString;
            SenderAddress = senderAddress;
            SubjectLine = subjectLine;
        }

        public CommunicationServiceEmailAttribute(string connectionString, string senderAddress, string subjectLine, string recipientAddresses)
        {
            ConnectionString = connectionString;
            SenderAddress = senderAddress;
            SubjectLine = subjectLine;
        }

        [AutoResolve]
        public string ConnectionString {get; set;}

        [AutoResolve]
        public string SenderAddress {get; set;}

        [AutoResolve]
        public string SubjectLine {get; set;}

        [AutoResolve]
        public string RecipientAddress {get; set;}
    }
}