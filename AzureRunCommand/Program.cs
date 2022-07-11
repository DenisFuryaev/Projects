using System;

namespace AzureRunCommand
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Error: You need to specify command by calling \"dotnet run -- \"");
                return;
            }

            try
            {
                // example: RunPowerShellScript -s script.txt -p params.json (commandID -s (script filename) -p (parameters filename))
                IDictionary<string, string> paramsDict = new Dictionary<string, string>();

                paramsDict.Add("commandID", args[0]);

                if (args.Length == 3)
                {
                    paramsDict.Add("-p", "AzureParameters.json");
                    paramsDict[args[1]] = args[2];
                }
                else if (args.Length == 5)
                {
                    paramsDict.Add(args[1], args[2]);
                    paramsDict.Add(args[3], args[4]);
                }
                else
                    throw new Exception("Error: invalid cmd params");

                foreach (var item in paramsDict)
                {
                    Console.WriteLine($"    {item.Key}: {item.Value}");
                }

                Azure.ReadAzureParameters(paramsDict["-p"]);

                await Azure.UpdateBearerToken();

                string runCommandOutput = await Azure.RunCommand(paramsDict["commandID"], paramsDict["-s"]);
                Console.WriteLine(runCommandOutput);

                string commandOutput = await Azure.GetCommandOutput();
                Console.WriteLine(commandOutput);
                
                //Azure.SaveAzureParameters();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
        }
    }
}