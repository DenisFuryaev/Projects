using Microsoft.AspNetCore.Mvc;
using AzureWebAPI.Services;

[ApiController]
[Route("[controller]")]
public class IPConfigController : ControllerBase
{
    private ILogger<IPConfigController> _logger;

    public IPConfigController(ILogger<IPConfigController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> RunCommand()
    {
        string runCommandOutput, commandOutput;
        try
        {
            runCommandOutput = await Azure.RunCommand("IPConfig");

            _logger.LogInformation("IPConfig command initiated succesfully at {DT}", DateTime.UtcNow.ToLocalTime().ToLongTimeString());

            commandOutput = await Azure.GetCommandOutput();
        }
        catch(Exception e)
        {
            _logger.LogWarning(e, "IPConfig command error occured at {DT}", DateTime.UtcNow.ToLocalTime().ToLongTimeString());
            return Conflict(e.Message);
        }

        _logger.LogInformation("IPConfig command completed succesfully at {DT}", DateTime.UtcNow.ToLocalTime().ToLongTimeString());
        return Ok(commandOutput);
    }
}
