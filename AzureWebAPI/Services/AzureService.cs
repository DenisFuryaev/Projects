using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using AzureWebAPI.Models;

namespace AzureWebAPI.Services
{
    public class Azure
    {
        static AzureParameters _azureParameters;
        static string _azureParametersFilename;
        static RequestBody body = new RequestBody();

        public static void ReadClientCredentials(ClientCredentials clientCredentials)
        {
            _azureParameters.ClientID = clientCredentials.ClientID;
            _azureParameters.ClientSecret = clientCredentials.ClientSecret;
        }

        public static async Task ReadAzureParameters(string filename)
        {
            _azureParametersFilename = filename;
            FileStream fileStream = File.Open(filename, FileMode.Open);
            _azureParameters = await JsonSerializer.DeserializeAsync<AzureParameters>(fileStream);
        }

        public static async Task SaveAzureParameters()
        {
            FileStream fileStream = File.Open(_azureParametersFilename, FileMode.Create);
            await JsonSerializer.SerializeAsync(fileStream, _azureParameters, _azureParameters.GetType());
        }

        public static async Task UpdateBearerToken()
        {
            string uri = $"https://login.microsoftonline.com/{_azureParameters.TenantID}/oauth2/token";

            var data = new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", _azureParameters.ClientID),
                new KeyValuePair<string, string>("client_secret", _azureParameters.ClientSecret),
                new KeyValuePair<string, string>("resource", _azureParameters.Resource)
            };

            using var client = new HttpClient();
            var response = client.PostAsync(uri, new FormUrlEncodedContent(data)).GetAwaiter().GetResult();
            
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                JsonNode responseBodyJson = JsonObject.Parse(responseBody);
                string bearerToken = responseBodyJson["access_token"].ToString();
                _azureParameters.Bearer = bearerToken; 
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }

            return;
        }

        #nullable enable
        public static async Task<string> RunCommand(string? commandID, ScriptBody? scriptBody = null)
        {
           
            var uri = $"https://management.azure.com/subscriptions/{_azureParameters.SubscriptionID}/resourceGroups/MyResourceGroup/providers/Microsoft.Compute/virtualMachines/myVM/runCommand?api-version=2022-03-01";

            string bodyJson;

            body.CommandId = commandID;
            if (scriptBody != null)
                body.Script = new string[] {scriptBody.Script};

            bodyJson = JsonSerializer.Serialize(body);

            var data = new StringContent(bodyJson, Encoding.UTF8, "application/json");
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _azureParameters.Bearer);

            var response = await client.PostAsync(uri, data);

            IEnumerable<string>? values;
            if (response.Headers.TryGetValues("Location", out values))
            {
                _azureParameters.Location = values.First();
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
            var uri = _azureParameters.Location;

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _azureParameters.Bearer);

            var response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                while (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    response = await client.GetAsync(uri);
                    await Task.Delay(5000);
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