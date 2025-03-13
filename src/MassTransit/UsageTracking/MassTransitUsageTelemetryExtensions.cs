#nullable enable
namespace MassTransit.UsageTracking;

using System;
using System.Linq;
using InMemoryTransport.Configuration;
using SqlTransport.Configuration;
using UsageTelemetry;


static class MassTransitUsageTelemetryExtensions
{
    public static EndpointUsageTelemetry? AddEndpoint<T>(this BusUsageTelemetry busUsageTelemetry, T configurator, MassTransitUsageTelemetry usageTelemetry)
        where T : IReceiveEndpointConfigurator
    {
        var path = configurator.InputAddress.AbsolutePath;
        var splitPath = path.Split('/');

        var endpointName = splitPath.LastOrDefault();
        var endpointType = MapEndpointType(configurator.GetType());

        if (splitPath.Length >= 3)
        {
            var skipCount = splitPath.Length - 3;
            splitPath = splitPath.Skip(skipCount).ToArray();

            if (splitPath[0].Equals("event-hub", StringComparison.OrdinalIgnoreCase))
            {
                endpointType = "EventHub";
                endpointName = splitPath[1];
            }

            if (splitPath[0].Equals("kafka", StringComparison.OrdinalIgnoreCase))
            {
                endpointType = "Kafka";
                endpointName = splitPath[1];
            }
        }

        if (endpointName is null)
            return null;

        if (endpointName.StartsWith("instance-") || endpointName.StartsWith("instance_"))
            return null;

        if (endpointName.Contains("_bus_") || endpointName.Contains("-bus-"))
            endpointName = "_bus_";

        var endpointUsage = new EndpointUsageTelemetry
        {
            Name = endpointName,
            Type = endpointType,
            PrefetchCount = configurator.PrefetchCount,
            ConcurrentMessageLimit = configurator.ConcurrentMessageLimit
        };

        if (endpointUsage.Type is "Kafka" or "EventHub")
        {
            usageTelemetry.Rider ??= [];
            var riderUsage = usageTelemetry.Rider.LastOrDefault(x => x.RiderType == endpointUsage.Type);
            if (riderUsage == null)
            {
                riderUsage = new RiderUsageTelemetry
                {
                    RiderType = endpointUsage.Type,
                    Endpoints = []
                };
                usageTelemetry.Rider.Add(riderUsage);
            }

            riderUsage.Endpoints?.Add(endpointUsage);
        }
        else
            busUsageTelemetry.Endpoints?.Add(endpointUsage);

        return endpointUsage;
    }

    static string MapEndpointType(Type type)
    {
        return type.Name switch
        {
            nameof(InMemoryReceiveEndpointConfiguration) => "InMemory",
            nameof(SqlReceiveEndpointConfiguration) => "SQL",
            "RabbitMqReceiveEndpointConfiguration" => "RabbitMQ",
            "ServiceBusReceiveEndpointConfiguration" => "ServiceBusQueue",
            "ServiceBusSubscriptionEndpointConfiguration" => "ServiceBusSubscription",
            "ActiveMqReceiveEndpointConfiguration" => "ActiveMQ",
            "AmazonSqsReceiveEndpointConfiguration" => "SQS",
            "KafkaTopicReceiveEndpointConfiguration~2" => "Kafka",
            "EventHubReceiveEndpointConfigurator~2" => "EventHub",
            _ => type.Name
        };
    }
}
