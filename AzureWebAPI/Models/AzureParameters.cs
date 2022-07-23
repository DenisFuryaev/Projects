namespace AzureWebAPI.Models
{
    public class AzureParameters
    {
        public string SubscriptionID {get; set;}
        public string TenantID {get; set;}
        public string ClientID {get; set;}
        public string ClientSecret {get; set;}
        public string Resource {get; set;}
        public string Bearer {get; set;}
        public string Location {get; set;}
    }
}