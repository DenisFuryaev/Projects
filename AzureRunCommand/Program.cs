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
                if (args.Length == 2)
                    Azure.ReadAzureParameters(args[1]);
                else
                    Azure.ReadAzureParameters("AzureParameters.json");

                await Azure.UpdateBearerToken();

                string runCommandOutput = await Azure.RunCommand(args[0]);
                Console.WriteLine(runCommandOutput);

                string commandOutput = await Azure.GetCommandOutput();
                Console.WriteLine(commandOutput);
                
                Azure.SaveAzureParameters();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
        }
    }
}