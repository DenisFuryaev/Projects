using Microsoft.AspNetCore.Mvc;
using AzureWebAPI.Services;
using AzureWebAPI.Models;
using System;

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

            // Client credentials:
            // {
            //    "ClientID": "e71a96b9-ff04-4367-93dd-01827236c9ae",
            //    "ClientSecret": "TPZ8Q~iUEmExyQQGpGXcw.kSrVyJxuYCQS1yzcZ4"
            // }
            Azure.ReadClientCredentials(clientCredentials);

            await Azure.UpdateBearerToken();
        }
        catch (Exception e)
        {
            _logger.LogError("Authorization error occured at {DT}", DateTime.UtcNow.ToLocalTime().ToLongTimeString());
            return Unauthorized(e.Message);
        }

        _logger.LogInformation("Authorization completed succesfully at {DT}", DateTime.UtcNow.ToLocalTime().ToLongTimeString());
        return Ok();
    }
}
