namespace MassTransit.Configuration
{
    public interface IInstanceConnector
    {
        IConsumerSpecification<TConsumer> CreateConsumerSpecification<TConsumer>()
            where TConsumer : class;

        ConnectHandle ConnectInstance(IConsumePipeConnector pipeConnector, object instance);

        ConnectHandle ConnectInstance<TInstance>(IConsumePipeConnector pipeConnector, TInstance instance,
            IConsumerSpecification<TInstance> specification)
            where TInstance : class;
    }
}
