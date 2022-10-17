using System;
using dotnet_first.Dtos;
using dotnet_first.Services.DotnetSecondService.Dtos;
using System.Net;
using System.Diagnostics;
using dotnet_first.Logging;
using OpenTelemetry.Resources;
using dotnet_first.Commons;

namespace dotnet_first.Services.ErrorService;

public interface IErrorService
{
    ResponseDto<CreateValueResponseDto> Run(
        string errorType
    );
}

public class ErrorService : IErrorService
{
    private readonly ActivitySource _source;

    public ErrorService()
    {
        _source = new ActivitySource(Constants.OTEL_SERVICE_NAME);
    }

    public ResponseDto<CreateValueResponseDto> Run(
        string errorType
    )
    {
        // Create span
        using var activity = _source.StartActivity($"{nameof(ErrorService)}.{nameof(Run)}");

        switch (errorType)
        {
            case "wait":
                return Wait(activity);
            default:
                return InternalServerError(activity);
        }
    }

    public ResponseDto<CreateValueResponseDto> Wait(
        Activity activity
    )
    {
        var message = "Everything's fine.";

        CustomLogger.Run(
            new CustomLog
            {
                ClassName = nameof(ErrorService),
                MethodName = nameof(InternalServerError),
                LogLevel = CustomLogLevel.INFO,
                Message = message,
                TraceId = activity?.TraceId.ToString(),
                SpanId = activity?.SpanId.ToString(),
            });

        return new ResponseDto<CreateValueResponseDto>
        {
            Message = message,
            StatusCode = HttpStatusCode.OK,
            Data = null,
        };
    }

    public ResponseDto<CreateValueResponseDto> Cpu(
        Activity activity
    )
    {
        var message = "Everything's fine.";

        var total = 0.0;
        for (var counter = 0; counter < 1000000; ++counter)
        {
            var temp = Math.Pow(counter, 5) * Math.Sqrt(counter);
            total += temp;
        }

        CustomLogger.Run(
            new CustomLog
            {
                ClassName = nameof(ErrorService),
                MethodName = nameof(InternalServerError),
                LogLevel = CustomLogLevel.INFO,
                Message = message,
                TraceId = activity?.TraceId.ToString(),
                SpanId = activity?.SpanId.ToString(),
            });

        return new ResponseDto<CreateValueResponseDto>
        {
            Message = message,
            StatusCode = HttpStatusCode.OK,
            Data = null,
        };
    }

    public ResponseDto<CreateValueResponseDto> Mem(
        Activity activity
    )
    {
        var message = "Everything's fine.";

        var array = new double[1000000000];

        CustomLogger.Run(
            new CustomLog
            {
                ClassName = nameof(ErrorService),
                MethodName = nameof(InternalServerError),
                LogLevel = CustomLogLevel.INFO,
                Message = message,
                TraceId = activity?.TraceId.ToString(),
                SpanId = activity?.SpanId.ToString(),
            });

        return new ResponseDto<CreateValueResponseDto>
        {
            Message = message,
            StatusCode = HttpStatusCode.OK,
            Data = null,
        };
    }

    public ResponseDto<CreateValueResponseDto> InternalServerError(
        Activity activity
    )
    {
        var message = "Task failed magnificently.";

        CustomLogger.Run(
            new CustomLog
            {
                ClassName = nameof(ErrorService),
                MethodName = nameof(InternalServerError),
                LogLevel = CustomLogLevel.ERROR,
                Message = message,
                TraceId = activity?.TraceId.ToString(),
                SpanId = activity?.SpanId.ToString(),
            });

        return new ResponseDto<CreateValueResponseDto>
        {
            Message = message,
            StatusCode = HttpStatusCode.InternalServerError,
            Data = null,
        };
    }
}

