using Microsoft.AspNetCore.Mvc;
using AzureWebAPI.Models;
using AzureWebAPI.Services;

[ApiController]
[Route("[controller]")]
public class RunPowerShellScriptController : ControllerBase
{
    public RunPowerShellScriptController()
    {

    }

    [HttpPost]
    public async Task<IActionResult> RunCommand(ScriptBody scriptBody)
    {
        string runCommandOutput, commandOutput;
        try
        {
            runCommandOutput = await Azure.RunCommand("RunPowerShellScript", scriptBody);

            commandOutput = await Azure.GetCommandOutput();
        }
        catch(Exception e)
        {
            return BadRequest(e.Message);
        }

        return Ok(commandOutput);
    }
}
