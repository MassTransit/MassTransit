namespace MassTransit.Configuration
{
    using Middleware;


    /// <summary>
    /// Configures a concurrency limit for a consumer, on the consumer configurator, which is constrained to
    /// the message types for that consumer, and only applies to the consumer prior to the consumer factory.
    /// </summary>
    /// <typeparam name="TSaga">The consumer type</typeparam>
    public class ConcurrencyLimitSagaConfigurationObserver<TSaga> :
        ISagaConfigurationObserver
        where TSaga : class, ISaga
    {
        readonly ISagaConfigurator<TSaga> _configurator;

        public ConcurrencyLimitSagaConfigurationObserver(ISagaConfigurator<TSaga> configurator, int concurrentMessageLimit, string id = null)
        {
            _configurator = configurator;
            Limiter = new ConcurrencyLimiter(concurrentMessageLimit, id);
        }

        public IConcurrencyLimiter Limiter { get; }

        void ISagaConfigurationObserver.SagaConfigured<T>(ISagaConfigurator<T> configurator)
        {
        }

        public void StateMachineSagaConfigured<TInstance>(ISagaConfigurator<TInstance> configurator, SagaStateMachine<TInstance> stateMachine)
            where TInstance : class, ISaga, SagaStateMachineInstance
        {
        }

        void ISagaConfigurationObserver.SagaMessageConfigured<T, TMessage>(ISagaMessageConfigurator<T, TMessage> configurator)
        {
            var specification = new ConcurrencyLimitConsumePipeSpecification<TMessage>(Limiter);

            _configurator.Message<TMessage>(x => x.AddPipeSpecification(specification));
        }
    }
}
