using Microsoft.AspNetCore.Mvc;
using AzureWebAPI.Models;
using AzureWebAPI.Services;

[ApiController]
[Route("[controller]")]
public class RunPowerShellScriptController : ControllerBase
{
    private ILogger<RunPowerShellScriptController> _logger;

    public RunPowerShellScriptController(ILogger<RunPowerShellScriptController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> RunCommand(ScriptBody scriptBody)
    {
        string runCommandOutput, commandOutput;
        try
        {
            runCommandOutput = await Azure.RunCommand("RunPowerShellScript", scriptBody);

            _logger.LogInformation("RunPowerShellScript command initiated succesfully");

            commandOutput = await Azure.GetCommandOutput();
        }
        catch(Exception e)
        {
            _logger.LogWarning(e, "RunPowerShellScript command error occured");            
            return BadRequest(e.Message);
        }

        _logger.LogInformation("RunPowerShellScript command completed succesfully");
        return Ok(commandOutput);
    }
}
