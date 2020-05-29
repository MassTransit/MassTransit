namespace MassTransit.Azure.ServiceBus.Core.Topology.Conventions
{
    public interface ISessionIdFormatter
    {
        string FormatSessionId<T>(SendContext<T> context)
            where T : class;
    }
}
