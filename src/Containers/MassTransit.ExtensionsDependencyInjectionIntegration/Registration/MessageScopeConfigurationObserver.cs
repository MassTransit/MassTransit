namespace MassTransit.ExtensionsDependencyInjectionIntegration.Registration
{
    using System;
    using ConsumeConfigurators;
    using ConsumerSpecifications;
    using Courier;
    using GreenPipes.Specifications;
    using PipeConfigurators;
    using Pipeline.Filters;
    using ScopeProviders;
    using Scoping;


    public class MessageScopeConfigurationObserver :
        ConfigurationObserver,
        IMessageConfigurationObserver
    {
        readonly IMessageScopeProvider _scopeProvider;
        readonly IServiceProvider _serviceProvider;

        public MessageScopeConfigurationObserver(IConsumePipeConfigurator receiveEndpointConfigurator, IServiceProvider serviceProvider)
            : base(receiveEndpointConfigurator)
        {
            _scopeProvider = new DependencyInjectionMessageScopeProvider(serviceProvider);
            _serviceProvider = serviceProvider;

            Connect(this);
        }

        public void MessageConfigured<TMessage>(IConsumePipeConfigurator configurator)
            where TMessage : class
        {
            var scopeFilter = new ScopeMessageFilter<TMessage>(_scopeProvider);
            var specification = new FilterPipeSpecification<ConsumeContext<TMessage>>(scopeFilter);

            configurator.AddPipeSpecification(specification);
        }

        public override void BatchConsumerConfigured<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, Batch<TMessage>> configurator)
        {
            var consumerSpecification = configurator as IConsumerMessageSpecification<TConsumer, Batch<TMessage>>;
            if (consumerSpecification == null)
                throw new ArgumentException("The configurator must be a consumer specification");

            var scopeFilter = new ScopeMessageFilter<Batch<TMessage>>(_scopeProvider);
            var specification = new FilterPipeSpecification<ConsumeContext<Batch<TMessage>>>(scopeFilter);

            consumerSpecification.AddPipeSpecification(specification);
        }

        public override void ActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator, Uri compensateAddress)
        {
            var scopeProvider = new DependencyInjectionExecuteActivityScopeProvider<TActivity, TArguments>(_serviceProvider);
            var scopeFilter = new ScopeExecuteFilter<TActivity, TArguments>(scopeProvider);
            var specification = new FilterPipeSpecification<ExecuteContext<TArguments>>(scopeFilter);

            configurator.Arguments(x => x.AddPipeSpecification(specification));
        }

        public override void ExecuteActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator)
        {
            var scopeProvider = new DependencyInjectionExecuteActivityScopeProvider<TActivity, TArguments>(_serviceProvider);
            var scopeFilter = new ScopeExecuteFilter<TActivity, TArguments>(scopeProvider);
            var specification = new FilterPipeSpecification<ExecuteContext<TArguments>>(scopeFilter);

            configurator.Arguments(x => x.AddPipeSpecification(specification));
        }

        public override void CompensateActivityConfigured<TActivity, TLog>(ICompensateActivityConfigurator<TActivity, TLog> configurator)
        {
            var scopeProvider = new DependencyInjectionCompensateActivityScopeProvider<TActivity, TLog>(_serviceProvider);
            var scopeFilter = new ScopeCompensateFilter<TActivity, TLog>(scopeProvider);
            var specification = new FilterPipeSpecification<CompensateContext<TLog>>(scopeFilter);

            configurator.Log(x => x.AddPipeSpecification(specification));
        }
    }
}
