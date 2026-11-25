using Microsoft.AspNetCore.Mvc;

namespace Carglass.TechnicalAssessment.Backend.Api.Controllers;

[ApiController]
[Route("healthcheck")]
public class HealthCheckController : ControllerBase
{
    [HttpGet("keepalive")]
    public IActionResult KeepAlive()
    {
        return Ok();
    }
}