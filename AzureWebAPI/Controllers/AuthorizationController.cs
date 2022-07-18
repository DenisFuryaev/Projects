using Microsoft.AspNetCore.Mvc;
using AzureWebAPI.Services;
using AzureWebAPI.Models;

[ApiController]
[Route("[controller]")]
public class AuthorizationController : ControllerBase
{

    public AuthorizationController()
    {

    }

    [HttpPost]
    public async Task<ActionResult> Authorize(ClientCredentials clientCredentials)
    {
        try
        {
            Azure.ReadAzureParameters("AzureParameters.json");

            // Client credentials:
            // {
            //    "clientID": "e71a96b9-ff04-4367-93dd-01827236c9ae",
            //    "clientSecret": "TPZ8Q~iUEmExyQQGpGXcw.kSrVyJxuYCQS1yzcZ4"
            // }
            Azure.ReadClientCredentials(clientCredentials);

            await Azure.UpdateBearerToken();
        }
        catch (Exception e)
        {
            return Conflict(e.Message);
        }

        return Ok();
    }
}
