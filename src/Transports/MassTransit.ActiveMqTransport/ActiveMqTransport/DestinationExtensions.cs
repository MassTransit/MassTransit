namespace MassTransit.ActiveMqTransport
{
    using Apache.NMS;
    using System;

    public static class DestinationExtensions
    {
        public static Uri? ToEndpointAddress(this IDestination destination)
        {
            return destination switch
            {
                null => null,
                IQueue queue => new Uri($"queue:{queue.QueueName}"),
                ITopic topic => new Uri($"topic:{topic.TopicName}"),
                _ => null
            };
        }
    }
}
