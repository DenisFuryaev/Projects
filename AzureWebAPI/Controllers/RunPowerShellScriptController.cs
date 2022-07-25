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

            _logger.LogInformation("RunPowerShellScript command initiated succesfully at {DT}", DateTime.UtcNow.ToLocalTime().ToLongTimeString());

            commandOutput = await Azure.GetCommandOutput();
        }
        catch(Exception e)
        {
            _logger.LogError("RunPowerShellScript command error occured at {DT}", DateTime.UtcNow.ToLocalTime().ToLongTimeString());            
            return BadRequest(e.Message);
        }

        _logger.LogInformation("RunPowerShellScript command completed succesfully at {DT}", DateTime.UtcNow.ToLocalTime().ToLongTimeString());
        return Ok(commandOutput);
    }
}
