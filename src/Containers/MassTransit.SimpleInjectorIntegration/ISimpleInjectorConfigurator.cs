namespace MassTransit
{
    using System;
    using SimpleInjector;

    public interface ISimpleInjectorConfigurator :
        IRegistrationConfigurator
    {
        Container Container { get; }

        /// <summary>
        /// Add the bus to the container, configured properly
        /// </summary>
        /// <param name="busFactory"></param>
        void AddBus(Func<IBusControl> busFactory);

        void AddRequestClient<T>(RequestTimeout timeout = default)
            where T : class;

        void AddRequestClient<T>(Uri destinationAddress, RequestTimeout timeout = default)
            where T : class;
    }
}
