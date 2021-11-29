namespace MassTransit.AzureServiceBusTransport
{
    public class EmptySessionIdFormatter :
        ISessionIdFormatter
    {
        string ISessionIdFormatter.FormatSessionId<T>(SendContext<T> context)
        {
            return null;
        }
    }
}
