namespace SiliconValve.Demo.CommunicationServiceEmail.Models
{
    public class AcsEmailMessage 
    {
        public string SenderAddress {get;set;}
        public string RecipientAddress {get;set;}
        public string HtmlEmailBody {get;set;}
        public string PlainTextEmailBody {get;set;}
        public string Subject {get;set;}
    }
}