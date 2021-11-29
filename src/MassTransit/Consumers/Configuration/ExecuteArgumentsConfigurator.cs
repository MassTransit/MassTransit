namespace MassTransit.Configuration
{
    public class ExecuteArgumentsConfigurator<TArguments> :
        IExecuteArgumentsConfigurator<TArguments>
        where TArguments : class
    {
        readonly IPipeConfigurator<ExecuteContext<TArguments>> _configurator;

        public ExecuteArgumentsConfigurator(IPipeConfigurator<ExecuteContext<TArguments>> configurator)
        {
            _configurator = configurator;
        }

        public void AddPipeSpecification(IPipeSpecification<ExecuteContext<TArguments>> specification)
        {
            _configurator.AddPipeSpecification(specification);
        }
    }
}
