using Microsoft.AspNetCore.Mvc;
using AzureWebAPI.Services;
using AzureWebAPI.Models;

[ApiController]
[Route("[controller]")]
public class AuthorizationController : ControllerBase
{
    private ILogger<AuthorizationController> _logger;

    public AuthorizationController(ILogger<AuthorizationController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult> Authorize(ClientCredentials clientCredentials)
    {
        try
        {
            await Azure.ReadAzureParameters("AzureParameters.json");
            
            Azure.ReadClientCredentials(clientCredentials);

            await Azure.UpdateBearerToken();
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Authorization error");
            return Unauthorized(e.Message);
        }

        _logger.LogInformation("Authorization completed succesfully");
        return Ok();
    }
}
