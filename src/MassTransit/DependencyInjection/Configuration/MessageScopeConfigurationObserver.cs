namespace MassTransit.Configuration
{
    using System;
    using DependencyInjection;
    using Middleware;


    public class MessageScopeConfigurationObserver :
        ConfigurationObserver,
        IMessageConfigurationObserver
    {
        readonly IServiceProvider _serviceProvider;

        public MessageScopeConfigurationObserver(IConsumePipeConfigurator receiveEndpointConfigurator, IServiceProvider serviceProvider)
            : base(receiveEndpointConfigurator)
        {
            _serviceProvider = serviceProvider;

            Connect(this);
        }

        public void MessageConfigured<TMessage>(IConsumePipeConfigurator configurator)
            where TMessage : class
        {
            var scopeProvider = new ConsumeScopeProvider(_serviceProvider);
            var scopeFilter = new ScopeMessageFilter<TMessage>(scopeProvider);
            var specification = new FilterPipeSpecification<ConsumeContext<TMessage>>(scopeFilter);

            configurator.AddPipeSpecification(specification);
        }

        public override void BatchConsumerConfigured<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, Batch<TMessage>> configurator)
        {
            var consumerSpecification = configurator as IConsumerMessageSpecification<TConsumer, Batch<TMessage>>;
            if (consumerSpecification == null)
                throw new ArgumentException("The configurator must be a consumer specification");

            var scopeProvider = new ConsumeScopeProvider(_serviceProvider);
            var scopeFilter = new ScopeMessageFilter<Batch<TMessage>>(scopeProvider);
            var specification = new FilterPipeSpecification<ConsumeContext<Batch<TMessage>>>(scopeFilter);

            consumerSpecification.AddPipeSpecification(specification);
        }

        public override void ActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator, Uri compensateAddress)
        {
            var scopeProvider = new ExecuteActivityScopeProvider<TActivity, TArguments>(_serviceProvider);
            var scopeFilter = new ScopeExecuteFilter<TActivity, TArguments>(scopeProvider);
            var specification = new FilterPipeSpecification<ExecuteContext<TArguments>>(scopeFilter);

            configurator.Arguments(x => x.AddPipeSpecification(specification));
        }

        public override void ExecuteActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator)
        {
            var scopeProvider = new ExecuteActivityScopeProvider<TActivity, TArguments>(_serviceProvider);
            var scopeFilter = new ScopeExecuteFilter<TActivity, TArguments>(scopeProvider);
            var specification = new FilterPipeSpecification<ExecuteContext<TArguments>>(scopeFilter);

            configurator.Arguments(x => x.AddPipeSpecification(specification));
        }

        public override void CompensateActivityConfigured<TActivity, TLog>(ICompensateActivityConfigurator<TActivity, TLog> configurator)
        {
            var scopeProvider = new CompensateActivityScopeProvider<TActivity, TLog>(_serviceProvider);
            var scopeFilter = new ScopeCompensateFilter<TActivity, TLog>(scopeProvider);
            var specification = new FilterPipeSpecification<CompensateContext<TLog>>(scopeFilter);

            configurator.Log(x => x.AddPipeSpecification(specification));
        }
    }
}
