using System.Net;
using System.Text;
using dotnet_first.Commons.Exceptions;
using dotnet_first.Controllers;
using dotnet_first.Dtos;
using dotnet_first.Logging;
using dotnet_first.Services.DotnetSecondService.Dtos;
using Google.Protobuf;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace dotnet_first.Services.DotnetSecondService;

public interface IDotnetSecondService
{
    Task<ResponseDto<CreateValueResponseDto>> Run(
        CreateValueRequestDto requestDto
    );
}

public class DotnetSecondService : IDotnetSecondService
{
    private const string DOTNET_SECOND_CREATE_URI =
        "http://dotnet-second.dotnet.svc.cluster.local:8080/dotnet/create";

    private readonly HttpClient _httpClient;

    public DotnetSecondService(
        IHttpClientFactory factory
    )
    {
        _httpClient = factory.CreateClient();
    }

    public async Task<ResponseDto<CreateValueResponseDto>> Run(
        CreateValueRequestDto requestDto
    )
    {
        try
        {
            // Parse request
            var requestDtoAsString = ParseRequestDto(requestDto);

            // Call second dotnet service
            var responseMessage = PerformHttpRequest(requestDtoAsString);

            // Parse response
            return await ParseResponseDto(responseMessage);
        }
        catch (ParsingFailedException e)
        {
            return new ResponseDto<CreateValueResponseDto>
            {
                Message = e.GetMessage(),
                StatusCode = HttpStatusCode.BadRequest,
                Data = null,
            };
        }
        catch (HttpRequestFailedException e)
        {
            return new ResponseDto<CreateValueResponseDto>
            {
                Message = e.GetMessage(),
                StatusCode = HttpStatusCode.InternalServerError,
                Data = null,
            };
        }
    }

    private string ParseRequestDto(
        CreateValueRequestDto requestDto
    )
    {
        try
        {
            LogParsingRequestDto();

            var requestDtoAsString = JsonConvert.SerializeObject(requestDto);

            LogParsingRequestDtoSuccessful();

            return requestDtoAsString;
        }
        catch (Exception e)
        {
            var message = "Request body is invalid.";
            LogParsingRequestDtoFailed(e, message);
            throw new ParsingFailedException(message);
        }
    }

    private HttpResponseMessage PerformHttpRequest(
        string requestDtoAsString
    )
    {
        try
        {
            LogPerformingHttpRequest();

            var stringContent = new StringContent(
            requestDtoAsString,
            Encoding.UTF8,
            "application/json"
        );

            var httpRequest = new HttpRequestMessage(
                HttpMethod.Post,
                DOTNET_SECOND_CREATE_URI
            )
            {
                Content = stringContent
            };

            var response = _httpClient.Send(httpRequest);

            LogPerformingHttpRequestSuccessful();

            return response;
        }
        catch (Exception e)
        {
            var message = "Performing HTTP request to second Dotnet Service is failed.";
            LogPerformingHttpRequestFailed(e, message);
            throw new HttpRequestFailedException(message);
        }
    }

    private async Task<ResponseDto<CreateValueResponseDto>> ParseResponseDto(
        HttpResponseMessage responseMessage
    )
    {
        try
        {
            LogParsingResponseDto();

            var responseAsString = await responseMessage.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<ResponseDto<CreateValueResponseDto>>(responseAsString);

            LogParsingResponseDtoSuccessful();

            return new ResponseDto<CreateValueResponseDto>
            {
                Message = "Value is created successfully.",
                StatusCode = response.StatusCode,
                Data = response.Data,
            };
        }
        catch (Exception e)
        {
            var message = "Response body could not be parsed.";
            LogParsingResponseDtoFailed(e, message);
            throw new ParsingFailedException(message);
        }
    }

    private void LogParsingRequestDto()
    {
        CustomLogger.Run(
            new CustomLog
            {
                ClassName = nameof(DotnetSecondService),
                MethodName = nameof(ParseRequestDto),
                LogLevel = CustomLogLevel.INFO,
                Message = "Parsing request DTO...",
            });
    }

    private void LogParsingRequestDtoSuccessful()
    {
        CustomLogger.Run(
            new CustomLog
            {
                ClassName = nameof(DotnetSecondService),
                MethodName = nameof(ParseRequestDto),
                LogLevel = CustomLogLevel.INFO,
                Message = "Parsing request DTO is successful.",
            });
    }

    private void LogParsingRequestDtoFailed(
        Exception e,
        string message
    )
    {
        CustomLogger.Run(
            new CustomLog
            {
                ClassName = nameof(DotnetSecondService),
                MethodName = nameof(ParseRequestDto),
                LogLevel = CustomLogLevel.ERROR,
                Message = message,
                Exception = e.Message,
                StackTrace = e.StackTrace,
            });
    }

    private void LogPerformingHttpRequest()
    {
        CustomLogger.Run(
            new CustomLog
            {
                ClassName = nameof(DotnetSecondService),
                MethodName = nameof(PerformHttpRequest),
                LogLevel = CustomLogLevel.INFO,
                Message = $"Performing HTTP request to second Dotnet Service...",
            });
    }

    private void LogPerformingHttpRequestSuccessful()
    {
        CustomLogger.Run(
            new CustomLog
            {
                ClassName = nameof(DotnetSecondService),
                MethodName = nameof(PerformHttpRequest),
                LogLevel = CustomLogLevel.INFO,
                Message = $"Performing HTTP request to second Dotnet Service is successful.",
            });
    }

    private void LogPerformingHttpRequestFailed(
        Exception e,
        string message
    )
    {
        CustomLogger.Run(
            new CustomLog
            {
                ClassName = nameof(DotnetSecondService),
                MethodName = nameof(PerformHttpRequest),
                LogLevel = CustomLogLevel.ERROR,
                Message = message,
                Exception = e.Message,
                StackTrace = e.StackTrace,
            });
    }

    private void LogParsingResponseDto()
    {
        CustomLogger.Run(
            new CustomLog
            {
                ClassName = nameof(DotnetSecondService),
                MethodName = nameof(ParseResponseDto),
                LogLevel = CustomLogLevel.INFO,
                Message = "Parsing response DTO...",
            });
    }

    private void LogParsingResponseDtoSuccessful()
    {
        CustomLogger.Run(
            new CustomLog
            {
                ClassName = nameof(DotnetSecondService),
                MethodName = nameof(ParseResponseDto),
                LogLevel = CustomLogLevel.INFO,
                Message = "Parsing response DTO is successful.",
            });
    }

    private void LogParsingResponseDtoFailed(
        Exception e,
        string message
    )
    {
        CustomLogger.Run(
            new CustomLog
            {
                ClassName = nameof(DotnetSecondService),
                MethodName = nameof(ParseResponseDto),
                LogLevel = CustomLogLevel.ERROR,
                Message = message,
                Exception = e.Message,
                StackTrace = e.StackTrace,
            });
    }
}

