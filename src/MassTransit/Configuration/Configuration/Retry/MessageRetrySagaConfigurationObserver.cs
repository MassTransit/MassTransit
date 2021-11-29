namespace MassTransit.Configuration
{
    using System;
    using System.Threading;
    using RetryPolicies;


    /// <summary>
    /// Configures a message retry for a saga, on the saga configurator, which is constrained to
    /// the message types for that saga, and only applies to the saga prior to the saga repository.
    /// </summary>
    /// <typeparam name="TSaga">The saga type</typeparam>
    public class MessageRetrySagaConfigurationObserver<TSaga> :
        ISagaConfigurationObserver
        where TSaga : class, ISaga
    {
        readonly CancellationToken _cancellationToken;
        readonly ISagaConfigurator<TSaga> _configurator;
        readonly Action<IRetryConfigurator> _configure;

        public MessageRetrySagaConfigurationObserver(ISagaConfigurator<TSaga> configurator, CancellationToken cancellationToken,
            Action<IRetryConfigurator> configure)
        {
            _configurator = configurator;
            _cancellationToken = cancellationToken;
            _configure = configure;
        }

        void ISagaConfigurationObserver.SagaConfigured<T>(ISagaConfigurator<T> configurator)
        {
        }

        public void StateMachineSagaConfigured<TInstance>(ISagaConfigurator<TInstance> configurator, SagaStateMachine<TInstance> stateMachine)
            where TInstance : class, ISaga, SagaStateMachineInstance
        {
        }

        void ISagaConfigurationObserver.SagaMessageConfigured<T, TMessage>(ISagaMessageConfigurator<T, TMessage> configurator)
        {
            var specification = new ConsumeContextRetryPipeSpecification<ConsumeContext<TMessage>, RetryConsumeContext<TMessage>>(Factory, _cancellationToken);

            _configure?.Invoke(specification);

            _configurator.Message<TMessage>(x => x.AddPipeSpecification(specification));
        }

        static RetryConsumeContext<TMessage> Factory<TMessage>(ConsumeContext<TMessage> context, IRetryPolicy retryPolicy, RetryContext retryContext)
            where TMessage : class
        {
            return new RetryConsumeContext<TMessage>(context, retryPolicy, retryContext);
        }
    }
}
