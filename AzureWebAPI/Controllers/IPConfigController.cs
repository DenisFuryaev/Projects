using Microsoft.AspNetCore.Mvc;
using AzureWebAPI.Services;

[ApiController]
[Route("[controller]")]
public class IPConfigController : ControllerBase
{
    public IPConfigController()
    {

    }

    [HttpGet]
    public async Task<IActionResult> RunCommand()
    {
        string runCommandOutput, commandOutput;
        try
        {
            runCommandOutput = await Azure.RunCommand("IPConfig");

            commandOutput = await Azure.GetCommandOutput();
        }
        catch(Exception e)
        {
            return Conflict(e.Message);
        }

        return Ok(commandOutput);
    }
}
