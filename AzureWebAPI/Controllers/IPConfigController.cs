using Microsoft.AspNetCore.Mvc;
using AzureWebAPI.Services;

[ApiController]
[Route("[controller]")]
public class IPConfigController : ControllerBase
{
    public IPConfigController()
    {

    }

    [HttpPost]
    public async Task<IActionResult> RunCommand()
    {
        string runCommandOutput = await Azure.RunCommand("IPConfig");

        string commandOutput = await Azure.GetCommandOutput();
        return Ok(commandOutput);
    }
}
