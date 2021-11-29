namespace MassTransit.Configuration
{
    using System;


    public class InMemoryOutboxConfigurationObserver :
        ConfigurationObserver,
        IMessageConfigurationObserver
    {
        readonly Action<IOutboxConfigurator> _configure;

        public InMemoryOutboxConfigurationObserver(IConsumePipeConfigurator configurator, Action<IOutboxConfigurator> configure)
            : base(configurator)
        {
            _configure = configure;

            Connect(this);
        }

        public void MessageConfigured<TMessage>(IConsumePipeConfigurator configurator)
            where TMessage : class
        {
            var specification = new InMemoryOutboxSpecification<TMessage>();

            _configure?.Invoke(specification);

            configurator.AddPipeSpecification(specification);
        }

        public override void BatchConsumerConfigured<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, Batch<TMessage>> configurator)
        {
            var specification = new InMemoryOutboxSpecification<Batch<TMessage>>();

            _configure?.Invoke(specification);

            configurator.Message(m => m.AddPipeSpecification(specification));
        }

        public override void ActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator, Uri compensateAddress)
        {
            var specification = new InMemoryExecuteContextOutboxSpecification<TArguments>();

            _configure?.Invoke(specification);

            configurator.Arguments(x => x.AddPipeSpecification(specification));
        }

        public override void ExecuteActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator)
        {
            var specification = new InMemoryExecuteContextOutboxSpecification<TArguments>();

            _configure?.Invoke(specification);

            configurator.Arguments(x => x.AddPipeSpecification(specification));
        }

        public override void CompensateActivityConfigured<TActivity, TLog>(ICompensateActivityConfigurator<TActivity, TLog> configurator)
        {
            var specification = new InMemoryCompensateContextOutboxSpecification<TLog>();

            _configure?.Invoke(specification);

            configurator.Log(x => x.AddPipeSpecification(specification));
        }
    }
}
