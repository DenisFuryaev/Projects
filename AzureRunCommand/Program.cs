using System.Diagnostics;

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
                Stopwatch stopwatch = new Stopwatch();

                paramsDict.Add("commandID", args[0]);
                paramsDict.Add("-p", "AzureParameters.json");

                if (args.Length == 3)
                {
                    paramsDict[args[1]] = args[2];
                }
                else if (args.Length == 5)
                {
                    paramsDict.Add(args[1], args[2]);
                    paramsDict.Add(args[3], args[4]);
                }
                else if (args.Length != 1)
                    throw new Exception("Error: invalid cmd params");

                foreach (var item in paramsDict)
                {
                    Console.WriteLine($"    {item.Key}: {item.Value}");
                }

                Azure.ReadAzureParameters(paramsDict["-p"]);

                await Azure.UpdateBearerToken();

                stopwatch.Start();

                string runCommandOutput;
                if (paramsDict.ContainsKey("-s"))
                    runCommandOutput = await Azure.RunCommand(paramsDict["commandID"], paramsDict["-s"]);
                else
                    runCommandOutput = await Azure.RunCommand(paramsDict["commandID"]);
                Console.WriteLine(runCommandOutput);

                string commandOutput = await Azure.GetCommandOutput();
                Console.WriteLine(commandOutput);

                stopwatch.Stop();
                Console.WriteLine($"Execution finished in {stopwatch.ElapsedMilliseconds} ms");
                
                //Azure.SaveAzureParameters();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
        }
    }
}