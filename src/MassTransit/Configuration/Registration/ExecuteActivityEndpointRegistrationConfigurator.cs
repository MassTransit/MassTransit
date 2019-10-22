namespace MassTransit.Registration
{
    using Courier;


    public class ExecuteActivityEndpointRegistrationConfigurator<TActivity, TArguments> :
        EndpointRegistrationConfigurator<IExecuteActivity<TArguments>>,
        IExecuteActivityEndpointRegistrationConfigurator<TActivity, TArguments>
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
    }
}
