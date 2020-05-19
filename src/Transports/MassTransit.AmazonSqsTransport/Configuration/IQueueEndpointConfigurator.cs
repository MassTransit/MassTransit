namespace MassTransit.AmazonSqsTransport
{
    public interface IQueueEndpointConfigurator :
        IQueueConfigurator
    {
        ushort PrefetchCount { set; }

        ushort WaitTimeSeconds { set; }

        bool PurgeOnStartup { set; }
    }
}
