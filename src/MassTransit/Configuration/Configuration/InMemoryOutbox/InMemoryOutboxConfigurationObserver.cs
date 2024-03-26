namespace MassTransit.Configuration
{
    using System;


    public class InMemoryOutboxConfigurationObserver :
        ConfigurationObserver,
        IMessageConfigurationObserver
    {
        readonly Action<IOutboxConfigurator> _configure;
        readonly ISetScopedConsumeContext _setter;

        public InMemoryOutboxConfigurationObserver(IRegistrationContext context, IConsumePipeConfigurator configurator, Action<IOutboxConfigurator> configure)
            : this(context as ISetScopedConsumeContext ?? throw new ArgumentException(nameof(context)), configurator, configure)
        {
        }

        public InMemoryOutboxConfigurationObserver(ISetScopedConsumeContext setter, IConsumePipeConfigurator configurator,
            Action<IOutboxConfigurator> configure)
            : base(configurator)
        {
            _setter = setter;
            _configure = configure;

            Connect(this);
        }

        public void MessageConfigured<TMessage>(IConsumePipeConfigurator configurator)
            where TMessage : class
        {
            var specification = new InMemoryOutboxSpecification<TMessage>(_setter);

            _configure?.Invoke(specification);

            configurator.AddPipeSpecification(specification);
        }

        public override void BatchConsumerConfigured<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, Batch<TMessage>> configurator)
        {
            var specification = new InMemoryOutboxSpecification<Batch<TMessage>>(_setter);

            _configure?.Invoke(specification);

            configurator.Message(m => m.AddPipeSpecification(specification));
        }

        public override void ActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator, Uri compensateAddress)
        {
            var specification = new InMemoryExecuteContextOutboxSpecification<TArguments>(_setter);

            _configure?.Invoke(specification);

            configurator.Arguments(x => x.AddPipeSpecification(specification));
        }

        public override void ExecuteActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator)
        {
            var specification = new InMemoryExecuteContextOutboxSpecification<TArguments>(_setter);

            _configure?.Invoke(specification);

            configurator.Arguments(x => x.AddPipeSpecification(specification));
        }

        public override void CompensateActivityConfigured<TActivity, TLog>(ICompensateActivityConfigurator<TActivity, TLog> configurator)
        {
            var specification = new InMemoryCompensateContextOutboxSpecification<TLog>(_setter);

            _configure?.Invoke(specification);

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
