using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using AzureWebAPI.Models;

namespace AzureWebAPI.Services
{
    public class Azure
    {
        static AzureParameters azureParameters;
        static string azureParametersFilename;
        static RequestBody body = new RequestBody();

        public static void ReadAzureParameters(string filename)
        {
            Console.WriteLine(filename);
            azureParametersFilename = filename;
            string jsonString = File.ReadAllText(filename);
            azureParameters = JsonSerializer.Deserialize<AzureParameters>(jsonString);
        }

        public static void SaveAzureParameters()
        {
            string jsonString = JsonSerializer.Serialize(azureParameters);
            File.WriteAllText(azureParametersFilename, jsonString);
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
                JsonNode responseBodyJson = JsonObject.Parse(responseBody);
                string bearerToken = responseBodyJson["access_token"].ToString();
                azureParameters.bearer = bearerToken; 
            }

            return;
        }

        #nullable enable
        public static async Task<string> RunCommand(string? commandID, string? scriptFilename = null)
        {
           
            var uri = $"https://management.azure.com/subscriptions/{azureParameters.subscriptionID}/resourceGroups/MyResourceGroup/providers/Microsoft.Compute/virtualMachines/myVM/runCommand?api-version=2022-03-01";

            string script, bodyJson;

            body.commandId = commandID;
            if (scriptFilename != null)
            {
                using(StreamReader r = new StreamReader(scriptFilename))
                {
                    script = r.ReadToEnd();
                }
                body.script = new string[] {script};
            }
            bodyJson = JsonSerializer.Serialize(body);

            var data = new StringContent(bodyJson, Encoding.UTF8, "application/json");
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

            throw new Exception(response.ReasonPhrase);
        }
        #nullable disable

        public static async Task<string> GetCommandOutput()
        {
            var uri = azureParameters.location;

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + azureParameters.bearer);

            var response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                while (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    response = await client.GetAsync(uri);
                    await Task.Delay(10000);
                }
                Console.WriteLine();

                string responseBody = await response.Content.ReadAsStringAsync();
                var jsonDocument = JsonDocument.Parse(responseBody);
                string message = jsonDocument.RootElement.GetProperty("value")[0].GetProperty("message").ToString();
                return message;
            }

            throw new Exception(response.ReasonPhrase);
        }
    }
}