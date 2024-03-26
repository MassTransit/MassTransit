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
        readonly ISetScopedConsumeContext _setScopedConsumeContext;

        public MessageScopeConfigurationObserver(IConsumePipeConfigurator receiveEndpointConfigurator, IServiceProvider serviceProvider)
            : this(receiveEndpointConfigurator, serviceProvider, LegacySetScopedConsumeContext.Instance)
        {
        }

        public MessageScopeConfigurationObserver(IConsumePipeConfigurator receiveEndpointConfigurator, IServiceProvider serviceProvider,
            ISetScopedConsumeContext setScopedConsumeContext)
            : base(receiveEndpointConfigurator)
        {
            _serviceProvider = serviceProvider;
            _setScopedConsumeContext = setScopedConsumeContext;

            Connect(this);
        }

        public void MessageConfigured<TMessage>(IConsumePipeConfigurator configurator)
            where TMessage : class
        {
            var scopeProvider = new ConsumeScopeProvider(_serviceProvider, _setScopedConsumeContext);
            var scopeFilter = new ScopeMessageFilter<TMessage>(scopeProvider);
            var specification = new FilterPipeSpecification<ConsumeContext<TMessage>>(scopeFilter);

            configurator.AddPipeSpecification(specification);
        }

        public override void BatchConsumerConfigured<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, Batch<TMessage>> configurator)
        {
            if (!(configurator is IConsumerMessageSpecification<TConsumer, Batch<TMessage>> consumerSpecification))
                throw new ArgumentException("The configurator must be a consumer specification");

            var scopeProvider = new ConsumeScopeProvider(_serviceProvider, _setScopedConsumeContext);
            var scopeFilter = new ScopeMessageFilter<Batch<TMessage>>(scopeProvider);
            var specification = new FilterPipeSpecification<ConsumeContext<Batch<TMessage>>>(scopeFilter);

            consumerSpecification.AddPipeSpecification(specification);
        }

        public override void ActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator, Uri compensateAddress)
        {
            var scopeProvider = new ExecuteActivityScopeProvider<TActivity, TArguments>(_serviceProvider, _setScopedConsumeContext);
            var scopeFilter = new ScopeExecuteFilter<TActivity, TArguments>(scopeProvider);
            var specification = new FilterPipeSpecification<ExecuteContext<TArguments>>(scopeFilter);

            configurator.Arguments(x => x.AddPipeSpecification(specification));
        }

        public override void ExecuteActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator)
        {
            var scopeProvider = new ExecuteActivityScopeProvider<TActivity, TArguments>(_serviceProvider, _setScopedConsumeContext);
            var scopeFilter = new ScopeExecuteFilter<TActivity, TArguments>(scopeProvider);
            var specification = new FilterPipeSpecification<ExecuteContext<TArguments>>(scopeFilter);

            configurator.Arguments(x => x.AddPipeSpecification(specification));
        }

        public override void CompensateActivityConfigured<TActivity, TLog>(ICompensateActivityConfigurator<TActivity, TLog> configurator)
        {
            var scopeProvider = new CompensateActivityScopeProvider<TActivity, TLog>(_serviceProvider, _setScopedConsumeContext);
            var scopeFilter = new ScopeCompensateFilter<TActivity, TLog>(scopeProvider);
            var specification = new FilterPipeSpecification<CompensateContext<TLog>>(scopeFilter);

            configurator.Log(x => x.AddPipeSpecification(specification));
        }

        public void Method4()
        {
        }

        public void Method5()
        {
        }

        public void Method6()
        {
        }
    }
}
