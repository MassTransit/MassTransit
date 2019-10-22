namespace MassTransit.Registration
{
    using Courier;


    public interface IExecuteActivityEndpointRegistrationConfigurator<TActivity, TArguments> :
        IEndpointRegistrationConfigurator
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
    }
}
