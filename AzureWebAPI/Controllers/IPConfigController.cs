using Microsoft.AspNetCore.Mvc;

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
