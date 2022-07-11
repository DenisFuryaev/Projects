using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AzureRunCommand
{
    public class Azure
    {
        static AzureParameters azureParameters;
        static string AzureParametersFilename;

        public static void ReadAzureParameters(string filename)
        {
            AzureParametersFilename = filename;
            using(StreamReader r = new StreamReader(AzureParametersFilename))
            {
                string jsonString = r.ReadToEnd();
                azureParameters = JsonConvert.DeserializeObject<AzureParameters>(jsonString);
            }
        }

        public static void SaveAzureParameters()
        {
            using(StreamWriter w = new StreamWriter(AzureParametersFilename))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(w, azureParameters);
            }
        }

        public static async Task UpdateBearerToken()
        {
            string uri = $"https://login.microsoftonline.com/{azureParameters.tenantID}/oauth2/token";

            var data = new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", azureParameters.clientID),
                new KeyValuePair<string, string>("client_secret", azureParameters.clientSecret),
                new KeyValuePair<string, string>("resource", azureParameters.resource)
            };

            using var client = new HttpClient();
            var response = client.PostAsync(uri, new FormUrlEncodedContent(data)).GetAwaiter().GetResult();
            
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                JObject responseBodyJson = JObject.Parse(responseBody);
                string bearerToken = responseBodyJson["access_token"].ToString();
                azureParameters.bearer = bearerToken; 
                return;
            }

            return;
        }

        public static async Task<string> RunCommand(string? commandID, string? scriptFilename = null)
        {
           
            var uri = $"https://management.azure.com/subscriptions/{azureParameters.subscriptionID}/resourceGroups/MyResourceGroup/providers/Microsoft.Compute/virtualMachines/myVM/runCommand?api-version=2022-03-01";

            string body, script;
            if (scriptFilename != null)
            {
                using(StreamReader r = new StreamReader(scriptFilename))
                {
                    script = r.ReadToEnd();
                }
                body = "{\"commandId\": \"" + commandID + "\", \"script\": [ \"" + script + "\"]}"; 
            }
            else
                body = "{\"commandId\": \"" + commandID + "\"}"; 

            var data = new StringContent(body, Encoding.UTF8, "application/json");
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + azureParameters.bearer);
            var response = await client.PostAsync(uri, data);

            IEnumerable<string>? values;
            if (response.Headers.TryGetValues("Location", out values))
            {
                azureParameters.location = values.First();
            }

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

            return response.ToString();
        }
        
        public static async Task<string> GetCommandOutput()
        {
            var uri = azureParameters.location;

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + azureParameters.bearer);
            var response = await client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Please wait for your command to finish execution in Azure");
                while (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    response = await client.GetAsync(uri);
                    Thread.Sleep(2000);
                    Console.Write("*");
                }
                Console.WriteLine();

                string responseBody = await response.Content.ReadAsStringAsync();
                return JObject.Parse(responseBody)["value"][0]["message"].ToString();
            }

            return response.StatusCode.ToString();
        }
    }
}