namespace MassTransit.Configuration
{
    public interface IMessageConsumeTopologyConvention<TMessage> :
        IMessageConsumeTopologyConvention
        where TMessage : class
    {
        bool TryGetMessageConsumeTopology(out IMessageConsumeTopology<TMessage> messageConsumeTopology);
    }


    public interface IMessageConsumeTopologyConvention
    {
        bool TryGetMessageConsumeTopologyConvention<T>(out IMessageConsumeTopologyConvention<T> convention)
            where T : class;
    }
}
