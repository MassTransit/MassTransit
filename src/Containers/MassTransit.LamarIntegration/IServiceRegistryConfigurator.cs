namespace MassTransit.LamarIntegration
{
    using System;
    using Lamar;


    public interface IServiceRegistryConfigurator :
        IRegistrationConfigurator
    {
        ServiceRegistry Builder { get; }

        /// <summary>
        /// Add the bus to the container, configured properly
        /// </summary>
        /// <param name="busFactory"></param>
        void AddBus(Func<IServiceContext, IBusControl> busFactory);
    }
}
