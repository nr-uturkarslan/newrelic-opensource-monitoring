using System.Net;
using dotnet_first.Dtos;
using dotnet_first.Logging;
using dotnet_first.Services.DotnetSecondService;
using dotnet_first.Services.DotnetSecondService.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_first.Controllers;

[ApiController]
[Route("dotnet")]
public class DotnetFirstController : ControllerBase
{
    private readonly IDotnetSecondService _dotnetSecondService;

    public DotnetFirstController(
        IDotnetSecondService dotnetSecondService
    )
    {
        _dotnetSecondService = dotnetSecondService;
    }

    [HttpPost(Name = "second")]
    [Route("second")]
    public async Task<ResponseDto<CreateValueResponseDto>> DotnetSecondMethod(
        [FromBody] CreateValueRequestDto requestDto
    )
    {
        LogFirstDotnetServiceTriggered();

        var response = await _dotnetSecondService.Run(requestDto);

        LogFirstDotnetServiceFinished();

        return response;
    }

    private void LogFirstDotnetServiceTriggered()
    {
        CustomLogger.Run(
            new CustomLog
            {
                ClassName = nameof(DotnetFirstController),
                MethodName = nameof(DotnetSecondMethod),
                LogLevel = CustomLogLevel.INFO,
                Message = $"First Dotnet Service is triggered...",
            });
    }

    private void LogFirstDotnetServiceFinished()
    {
        CustomLogger.Run(
            new CustomLog
            {
                ClassName = nameof(DotnetFirstController),
                MethodName = nameof(DotnetSecondMethod),
                LogLevel = CustomLogLevel.INFO,
                Message = $"First Dotnet Service is finished...",
            });
    }
}

