using Microsoft.AspNetCore.Mvc;
using AzureWebAPI.Models;

[ApiController]
[Route("[controller]")]
public class RunPowerShellCommandController : ControllerBase
{
    public RunPowerShellCommandController()
    {

    }

    [HttpPost]
    public IActionResult RunCommand(ScriptBody scriptBody)
    {
        return Ok();
    }
}
