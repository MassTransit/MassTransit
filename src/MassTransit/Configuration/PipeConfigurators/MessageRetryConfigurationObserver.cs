namespace MassTransit.PipeConfigurators
{
    using System;
    using System.Threading;
    using ConsumeConfigurators;
    using Context;
    using GreenPipes;
    using GreenPipes.Configurators;


    public class MessageRetryConfigurationObserver :
        ConfigurationObserver,
        IMessageConfigurationObserver
    {
        readonly CancellationToken _cancellationToken;
        readonly Action<IRetryConfigurator> _configure;

        public MessageRetryConfigurationObserver(IConsumePipeConfigurator receiveEndpointConfigurator, CancellationToken cancellationToken,
            Action<IRetryConfigurator> configure)
            : base(receiveEndpointConfigurator)
        {
            _cancellationToken = cancellationToken;
            _configure = configure;

            Connect(this);
        }

        public void MessageConfigured<TMessage>(IConsumePipeConfigurator configurator)
            where TMessage : class
        {
            var specification = new ConsumeContextRetryPipeSpecification<ConsumeContext<TMessage>, RetryConsumeContext<TMessage>>(Factory, _cancellationToken);

            _configure?.Invoke(specification);

            configurator.AddPipeSpecification(specification);
        }

        public override void ActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator, Uri compensateAddress)
        {
            var specification = new ExecuteContextRetryPipeSpecification<TArguments>(_cancellationToken);

            _configure?.Invoke(specification);

            configurator.Arguments(x => x.AddPipeSpecification(specification));
        }

        public override void ExecuteActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator)
        {
            var specification = new ExecuteContextRetryPipeSpecification<TArguments>(_cancellationToken);

            _configure?.Invoke(specification);

            configurator.Arguments(x => x.AddPipeSpecification(specification));
        }

        public override void CompensateActivityConfigured<TActivity, TLog>(ICompensateActivityConfigurator<TActivity, TLog> configurator)
        {
            var specification = new CompensateContextRetryPipeSpecification<TLog>(_cancellationToken);

            _configure?.Invoke(specification);

            configurator.Log(x => x.AddPipeSpecification(specification));
        }

        static RetryConsumeContext<TMessage> Factory<TMessage>(ConsumeContext<TMessage> context, IRetryPolicy retryPolicy, RetryContext retryContext)
            where TMessage : class
        {
            return new RetryConsumeContext<TMessage>(context, retryPolicy, retryContext);
        }
    }
}
