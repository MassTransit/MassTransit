namespace MassTransit.Configuration
{
    using System;


    public class TimeoutConfigurationObserver :
        ConfigurationObserver,
        IMessageConfigurationObserver
    {
        readonly Action<ITimeoutConfigurator> _configure;

        public TimeoutConfigurationObserver(IConsumePipeConfigurator configurator, Action<ITimeoutConfigurator> configure)
            : base(configurator)
        {
            _configure = configure;

            Connect(this);
        }

        public void MessageConfigured<TMessage>(IConsumePipeConfigurator configurator)
            where TMessage : class
        {
            var specification = new TimeoutSpecification<TMessage>();

            _configure?.Invoke(specification);

            configurator.AddPipeSpecification(specification);
        }

        public override void BatchConsumerConfigured<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, Batch<TMessage>> configurator)
        {
            var specification = new TimeoutSpecification<Batch<TMessage>>();

            _configure?.Invoke(specification);

            configurator.Message(m => m.AddPipeSpecification(specification));
        }

        public override void ActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator, Uri compensateAddress)
        {
            var specification = new ExecuteContextTimeoutSpecification<TArguments>();

            _configure?.Invoke(specification);

            configurator.Arguments(x => x.AddPipeSpecification(specification));
        }

        public override void ExecuteActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator)
        {
            var specification = new ExecuteContextTimeoutSpecification<TArguments>();

            _configure?.Invoke(specification);

            configurator.Arguments(x => x.AddPipeSpecification(specification));
        }

        public override void CompensateActivityConfigured<TActivity, TLog>(ICompensateActivityConfigurator<TActivity, TLog> configurator)
        {
            var specification = new CompensateContextTimeoutSpecification<TLog>();

            _configure?.Invoke(specification);

            configurator.Log(x => x.AddPipeSpecification(specification));
        }
    }
}
