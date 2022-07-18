namespace AzureWebAPI.Models
{
    public class AzureParameters
    {
        public string subscriptionID {get; set;}
        public string tenantID {get; set;}
        public string clientID {get; set;}
        public string clientSecret {get; set;}
        public string resource {get; set;}
        public string bearer {get; set;}
        public string location {get; set;}
    }
}