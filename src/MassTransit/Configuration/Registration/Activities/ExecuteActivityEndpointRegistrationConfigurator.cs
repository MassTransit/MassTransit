namespace MassTransit.Registration
{
    using Courier;


    public class ExecuteActivityEndpointRegistrationConfigurator<TActivity, TArguments> :
        EndpointRegistrationConfigurator<IExecuteActivity<TArguments>>,
        IExecuteActivityEndpointRegistrationConfigurator
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        public ExecuteActivityEndpointRegistrationConfigurator()
        {
            ConfigureConsumeTopology = false;
        }
    }
}
