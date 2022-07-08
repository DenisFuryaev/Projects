using System;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AzureRunCommand
{
    public class Azure
    {
        static string[] bearer = {"Bearer ", ""};
        static string subscriptionID = "9d49597d-c13e-40f9-b716-e9a16661e8e4";
        static string tenantID = "cedbfdbf-3892-4452-9480-04de054ba204";
        static string clientID = "e7072e75-0219-4f97-a218-b5efcb2ee4a5";
        static string clientSecret = "YxA8Q~xhtFhMjxFftl6CLN8gYmdiAV~rVglTEchI";
        static string? location;

        public static async Task UpdateBearerToken()
        {
            string uri = $"https://login.microsoftonline.com/{tenantID}/oauth2/token";

            var data = new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", clientID),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                new KeyValuePair<string, string>("resource", "https://management.azure.com")
            };

            using var client = new HttpClient();
            var response = client.PostAsync(uri, new FormUrlEncodedContent(data)).GetAwaiter().GetResult();
            
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                JObject responseBodyJson = JObject.Parse(responseBody);
                string bearerToken = responseBodyJson["access_token"].ToString();
                bearer[1] = bearerToken; 
                return;
            }

            return;
        }
        public static async Task<string> RunCommand(string? command)
        {
           
            var uri = $"https://management.azure.com/subscriptions/{subscriptionID}/resourceGroups/MyResourceGroup/providers/Microsoft.Compute/virtualMachines/myVM/runCommand?api-version=2022-03-01";
            var body = "{\"commandId\": \"" + command + "\"}"; 

            var data = new StringContent(body, Encoding.UTF8, "application/json");
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", bearer[0] + bearer[1]);
            var response = await client.PostAsync(uri, data);

            IEnumerable<string>? values;
            if (response.Headers.TryGetValues("Location", out values))
            {
                location = values.First();
            }

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

            return response.ToString();
        }
        public static async Task<string> GetCommandOutput()
        {
            var uri = location;

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", bearer[0] + bearer[1]);
            var response = await client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Please wait for your command to finish execution in Azure");
                while (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    response = await client.GetAsync(uri);
                    Thread.Sleep(1000);
                    Console.Write("*");
                }
                Console.WriteLine();

                string responseBody = await response.Content.ReadAsStringAsync();
                return JObject.Parse(responseBody)["value"][0]["message"].ToString();
            }

            return response.StatusCode.ToString();
        }
    }

    public class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                Console.Write("Input command to run in Azure: ");
                string? command = Console.ReadLine();

                await Azure.UpdateBearerToken();

                string runCommandOutput = await Azure.RunCommand(command);
                Console.WriteLine(runCommandOutput);

                string commandOutput = await Azure.GetCommandOutput();
                Console.WriteLine(commandOutput);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
        }
    }
}