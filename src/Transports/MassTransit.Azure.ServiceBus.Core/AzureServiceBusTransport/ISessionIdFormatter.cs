namespace MassTransit.AzureServiceBusTransport
{
    public interface ISessionIdFormatter
    {
        string FormatSessionId<T>(SendContext<T> context)
            where T : class;
    }
}
