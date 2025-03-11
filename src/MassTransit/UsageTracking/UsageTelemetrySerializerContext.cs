namespace MassTransit.UsageTracking;

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using UsageTelemetry;


[JsonSerializable(typeof(Guid?))]
[JsonSerializable(typeof(Guid))]
[JsonSerializable(typeof(int?))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(MassTransitUsageTelemetry))]
[JsonSerializable(typeof(HostUsageTelemetry))]
[JsonSerializable(typeof(BusUsageTelemetry))]
[JsonSerializable(typeof(RiderUsageTelemetry))]
[JsonSerializable(typeof(EndpointUsageTelemetry))]
[JsonSerializable(typeof(List<BusUsageTelemetry>))]
[JsonSerializable(typeof(List<RiderUsageTelemetry>))]
[JsonSerializable(typeof(List<EndpointUsageTelemetry>))]
[JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault, PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower)]
public partial class UsageTelemetrySerializerContext :
    JsonSerializerContext;
