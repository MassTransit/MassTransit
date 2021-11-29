namespace MassTransit.Configuration
{
    using Courier;


    public class ExecuteActivityArgumentsConfigurator<TActivity, TArguments> :
        IExecuteActivityArgumentsConfigurator<TArguments>
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly IPipeConfigurator<ExecuteActivityContext<TActivity, TArguments>> _configurator;

        public ExecuteActivityArgumentsConfigurator(IPipeConfigurator<ExecuteActivityContext<TActivity, TArguments>> configurator)
        {
            _configurator = configurator;
        }

        public void AddPipeSpecification(IPipeSpecification<ExecuteActivityContext<TArguments>> specification)
        {
            _configurator.AddPipeSpecification(specification);
        }
    }
}
