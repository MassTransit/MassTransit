namespace MassTransit.SimpleInjectorIntegration
{
    using System;
    using SimpleInjector;


    public interface ISimpleInjectorConfigurator :
        IRegistrationConfigurator<Container>
    {
        Container Container { get; }

        /// <summary>
        /// Add the bus to the container, configured properly
        /// </summary>
        /// <param name="busFactory"></param>
        void AddBus(Func<IBusControl> busFactory);
    }
}
