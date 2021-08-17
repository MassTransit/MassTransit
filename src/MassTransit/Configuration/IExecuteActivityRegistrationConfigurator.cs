namespace MassTransit
{
    using System;
    using Courier;
    using Registration;


    public interface IExecuteActivityRegistrationConfigurator<TActivity, TArguments> :
        IExecuteActivityRegistrationConfigurator
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
    }


    public interface IExecuteActivityRegistrationConfigurator
    {
        void Endpoint(Action<IExecuteActivityEndpointRegistrationConfigurator> configure);
    }
}
