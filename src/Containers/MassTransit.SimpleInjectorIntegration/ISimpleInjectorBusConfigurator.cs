namespace MassTransit.SimpleInjectorIntegration
{
    using System;
    using SimpleInjector;


    public interface ISimpleInjectorBusConfigurator :
        IBusRegistrationConfigurator
    {
        Container Container { get; }

        /// <summary>
        /// Add the bus to the container, configured properly
        /// </summary>
        /// <param name="busFactory"></param>
        void AddBus(Func<IBusControl> busFactory);
    }
}
