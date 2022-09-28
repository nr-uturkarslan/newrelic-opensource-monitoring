using System.Text;
using dotnet_first.Dtos;
using dotnet_first.Services.DotnetSecondService.Dtos;
using Newtonsoft.Json;

namespace dotnet_first.Services.DotnetSecondService;

public class DotnetSecondService
{
    private const string DOTNET_SECOND_CREATE_URI =
        "http://dotnet-second.dotnet.svc.cluster.local:8080/dotnet/create";

    private readonly ILogger<DotnetSecondService> _logger;

    private readonly HttpClient _httpClient;

    public DotnetSecondService(
        ILogger<DotnetSecondService> logger,
        IHttpClientFactory factory
    )
    {
        _logger = logger;

        _httpClient = factory.CreateClient();
    }

    public async Task<CreateValueResponseDto> Run(
        CreateValueRequestDto requestDto
    )
    {
        var requestDtoAsString = ParseRequestDto(requestDto);
        var responseMessage = PerformHttpRequest(requestDtoAsString);
        return await ParseResponseMessage(responseMessage);
    }

    private string ParseRequestDto(
        CreateValueRequestDto requestDto
    )
    {
        _logger.LogInformation("Parsing request DTO...");

        var requestDtoAsString = JsonConvert.SerializeObject(requestDto);

        _logger.LogInformation("Request DTO is parsed successfully");

        return requestDtoAsString;
    }

    private HttpResponseMessage PerformHttpRequest(
        string requestDtoAsString
    )
    {
        _logger.LogInformation("Performing web request...");

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

        _logger.LogInformation("Web request is performed successfully");

        return response;
    }

    private async Task<CreateValueResponseDto> ParseResponseMessage(
        HttpResponseMessage responseMessage
    )
    {
        _logger.LogInformation("Parsing response DTO...");
        var responseBody = await responseMessage.Content.ReadAsStringAsync();

        _logger.LogInformation($"Response body: {responseBody}");

        var responseDto = JsonConvert.DeserializeObject<ResponseDto<CreateValueResponseDto>>(responseBody);
        _logger.LogInformation("Response DTO is parsed successfully");

        return responseDto.Data;
    }
}

