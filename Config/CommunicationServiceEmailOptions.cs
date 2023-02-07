
namespace SiliconValve.Demo.CommunicationServiceEmail
{
    public class CommunicationServiceEmailOptions
    {
        public string ConnectionString {get;set;}
        public string SenderAddress {get;set;}
        public string RecipientAddress {get;set;}
        public string HtmlEmailBody {get;set;}
        public string PlainTextEmailBody {get;set;}
        public string Subject {get;set;}
    }
}