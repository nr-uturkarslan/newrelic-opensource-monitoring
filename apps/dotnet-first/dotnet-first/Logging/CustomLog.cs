using System;
using dotnet_first.Commons;
using Newtonsoft.Json;

namespace dotnet_first.Logging;

public class CustomLog
{
    [JsonProperty("service.name")]
    public string ServiceName { get; set; } = Constants.OTEL_SERVICE_NAME;

    [JsonProperty("className")]
    public string ClassName { get; set; }

    [JsonProperty("methodName")]
    public string MethodName { get; set; }

    [JsonProperty("logLevel")]
    public string LogLevel { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }

    [JsonProperty("exception")]
    public string Exception { get; set; }

    [JsonProperty("stackTrace")]
    public string StackTrace { get; set; }
}

