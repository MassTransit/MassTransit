namespace MassTransit.AmazonSqsTransport
{
    public interface IQueueEndpointConfigurator :
        IQueueConfigurator
    {
        ushort WaitTimeSeconds { set; }

        bool PurgeOnStartup { set; }
    }
}
