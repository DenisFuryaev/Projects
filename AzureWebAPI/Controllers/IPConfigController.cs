using Microsoft.AspNetCore.Mvc;

namespace AzureWebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class IPConfigController : ControllerBase
{
    public IPConfigController()
    {

    }

    [HttpPost]
    public IActionResult RunCommand()
    {
        return Ok();
    }
}
