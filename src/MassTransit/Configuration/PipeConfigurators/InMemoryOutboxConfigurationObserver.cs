namespace MassTransit.PipeConfigurators
{
    using System;
    using ConsumeConfigurators;


    public class InMemoryOutboxConfigurationObserver :
        ConfigurationObserver,
        IMessageConfigurationObserver
    {
        public InMemoryOutboxConfigurationObserver(IConsumePipeConfigurator configurator)
            : base(configurator)
        {
            Connect(this);
        }

        public override void ActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator, Uri compensateAddress)
        {
            var specification = new InMemoryExecuteContextOutboxSpecification<TArguments>();

            configurator.Arguments(x => x.AddPipeSpecification(specification));
        }

        public override void ExecuteActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator)
        {
            var specification = new InMemoryExecuteContextOutboxSpecification<TArguments>();

            configurator.Arguments(x => x.AddPipeSpecification(specification));
        }

        public override void CompensateActivityConfigured<TActivity, TLog>(ICompensateActivityConfigurator<TActivity, TLog> configurator)
        {
            var specification = new InMemoryCompensateContextOutboxSpecification<TLog>();

            configurator.Log(x => x.AddPipeSpecification(specification));
        }

        public void MessageConfigured<TMessage>(IConsumePipeConfigurator configurator)
            where TMessage : class
        {
            var specification = new InMemoryOutboxSpecification<TMessage>();

            configurator.AddPipeSpecification(specification);
        }
    }
}
