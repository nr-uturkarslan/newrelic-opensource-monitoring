using dotnet_first.Services.DotnetSecondService.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_first.Controllers;

[ApiController]
[Route("dotnet")]
public class DotnetFirstController : ControllerBase
{
    private readonly ILogger<DotnetFirstController> _logger;

    public DotnetFirstController(
        ILogger<DotnetFirstController> logger
    )
    {
        _logger = logger;
    }

    [HttpPost(Name = "second")]
    [Route("second")]
    public ActionResult DotnetSecondMethod(
        [FromBody] CreateValueRequestDto requestDto
    )
    {
        var result = new ObjectResult("Test");
        //result.StatusCode = (int)response.StatusCode;
        result.StatusCode = 200;
        return result;
    }
}

