namespace MassTransit
{
    using System;


    public class HangfireEndpointOptions
    {
        public const string DefaultQueueName = "mt-message-queue";

        public int? PrefetchCount { get; set; } = Environment.ProcessorCount;
        public int? ConcurrentMessageLimit { get; set; }
        public string QueueName { get; set; } = "hangfire";
    }
}
