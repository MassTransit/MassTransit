namespace MassTransit.KafkaIntegration.Subscriptions
{
    using Context;
    using GreenPipes;
    using Registration;


    public interface IKafkaSubscriptionDefinition :
        ISpecification
    {
        string Topic { get; }
        bool IsAutoCommitEnabled { get; }
        ILogContext LogContext { get; }
        IKafkaSubscription Build(IBusInstance busInstance);
    }
}
