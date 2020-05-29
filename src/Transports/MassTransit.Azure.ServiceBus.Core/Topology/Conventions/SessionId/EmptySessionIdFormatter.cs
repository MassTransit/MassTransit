namespace MassTransit.Azure.ServiceBus.Core.Topology.Conventions.SessionId
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
