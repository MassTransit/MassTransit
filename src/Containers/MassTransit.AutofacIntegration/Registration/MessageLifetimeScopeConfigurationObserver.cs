namespace MassTransit.AutofacIntegration.Registration
{
    using System;
    using Autofac;
    using ConsumeConfigurators;
    using ConsumerSpecifications;
    using Courier;
    using GreenPipes.Specifications;
    using PipeConfigurators;
    using Pipeline.Filters;
    using ScopeProviders;


    public class MessageLifetimeScopeConfigurationObserver :
        ConfigurationObserver,
        IMessageConfigurationObserver
    {
        readonly Action<ContainerBuilder, ConsumeContext> _configureScope;
        readonly string _name;
        readonly ILifetimeScopeProvider _scopeProvider;

        public MessageLifetimeScopeConfigurationObserver(IConsumePipeConfigurator receiveEndpointConfigurator, ILifetimeScopeProvider scopeProvider,
            string name, Action<ContainerBuilder, ConsumeContext> configureScope)
            : base(receiveEndpointConfigurator)
        {
            _scopeProvider = scopeProvider;
            _name = name;
            _configureScope = configureScope;

            Connect(this);
        }

        public void MessageConfigured<TMessage>(IConsumePipeConfigurator configurator)
            where TMessage : class
        {
            var scopeFilter = new ScopeMessageFilter<TMessage>(new AutofacMessageScopeProvider(_scopeProvider, _name, _configureScope));
            var specification = new FilterPipeSpecification<ConsumeContext<TMessage>>(scopeFilter);

            configurator.AddPipeSpecification(specification);
        }

        public override void BatchConsumerConfigured<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, Batch<TMessage>> configurator)
        {
            var consumerSpecification = configurator as IConsumerMessageSpecification<TConsumer, Batch<TMessage>>;
            if (consumerSpecification == null)
                throw new ArgumentException("The configurator must be a consumer specification");

            var scopeFilter = new ScopeMessageFilter<Batch<TMessage>>(new AutofacMessageScopeProvider(_scopeProvider, _name, _configureScope));
            var specification = new FilterPipeSpecification<ConsumeContext<Batch<TMessage>>>(scopeFilter);

            consumerSpecification.AddPipeSpecification(specification);
        }

        public override void ActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator, Uri compensateAddress)
        {
            var scopeProvider = new AutofacExecuteActivityScopeProvider<TActivity, TArguments>(_scopeProvider, _name, _configureScope);
            var scopeFilter = new ScopeExecuteFilter<TActivity, TArguments>(scopeProvider);
            var specification = new FilterPipeSpecification<ExecuteContext<TArguments>>(scopeFilter);

            configurator.Arguments(x => x.AddPipeSpecification(specification));
        }

        public override void ExecuteActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator)
        {
            var scopeProvider = new AutofacExecuteActivityScopeProvider<TActivity, TArguments>(_scopeProvider, _name, _configureScope);
            var scopeFilter = new ScopeExecuteFilter<TActivity, TArguments>(scopeProvider);
            var specification = new FilterPipeSpecification<ExecuteContext<TArguments>>(scopeFilter);

            configurator.Arguments(x => x.AddPipeSpecification(specification));
        }

        public override void CompensateActivityConfigured<TActivity, TLog>(ICompensateActivityConfigurator<TActivity, TLog> configurator)
        {
            var scopeProvider = new AutofacCompensateActivityScopeProvider<TActivity, TLog>(_scopeProvider, _name, _configureScope);
            var scopeFilter = new ScopeCompensateFilter<TActivity, TLog>(scopeProvider);
            var specification = new FilterPipeSpecification<CompensateContext<TLog>>(scopeFilter);

            configurator.Log(x => x.AddPipeSpecification(specification));
        }
    }
}
