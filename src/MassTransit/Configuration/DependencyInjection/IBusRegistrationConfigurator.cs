namespace MassTransit
{
    using System;
    using Configuration;


    /// <summary>
    /// Configures the container registration, and supports creation of a bus or a mediator.
    /// </summary>
    public interface IBusRegistrationConfigurator :
        IRegistrationConfigurator
    {
        IContainerRegistrar Registrar { get; }

        /// <summary>
        /// This method is being deprecated. Use the transport-specific UsingRabbitMq, UsingActiveMq, etc. methods instead.
        /// </summary>
        /// <param name="busFactory"></param>
        [Obsolete("Use 'Using[TransportName]' instead. Visit https://masstransit.io/obsolete for details.")]
        void AddBus(Func<IBusRegistrationContext, IBusControl> busFactory);

        /// <summary>
        /// Sets the bus factory. This is used by the transport extension methods (such as UsingRabbitMq, UsingActiveMq, etc.) to
        /// specify the bus factory. The extension method approach is preferred (since v7) over the AddBus method.
        /// </summary>
        /// <param name="busFactory"></param>
        /// <typeparam name="T"></typeparam>
        void SetBusFactory<T>(T busFactory)
            where T : class, IRegistrationBusFactory;

        /// <summary>
        /// Add bus rider
        /// </summary>
        /// <param name="configure"></param>
        void AddRider(Action<IRiderRegistrationConfigurator> configure);

        /// <summary>
        /// Adds a method that is called for each receive endpoint when it is configured, allowing additional
        /// configuration to be specified. Multiple callbacks may be configured.
        /// </summary>
        /// <param name="callback">Callback invoked for each receive endpoint</param>
        void AddConfigureEndpointsCallback(ConfigureEndpointsCallback callback);

        /// <summary>
        /// Adds a method that is called for each receive endpoint when it is configured, allowing additional
        /// configuration to be specified. Multiple callbacks may be configured.
        /// </summary>
        /// <param name="callback">Callback invoked for each receive endpoint</param>
        void AddConfigureEndpointsCallback(ConfigureEndpointsProviderCallback callback);

        /// <summary>
        /// Override the default request client factory to enable advanced scenarios. This is typically not called by end-users.
        /// </summary>
        /// <param name="clientFactory"></param>
        void SetRequestClientFactory(Func<IBus, RequestTimeout, IClientFactory> clientFactory);
    }


    /// <summary>
    /// Configures additional bus instances, configured via MultiBus
    /// </summary>
    /// <typeparam name="TBus">The additional bus interface type</typeparam>
    public interface IBusRegistrationConfigurator<in TBus> :
        IBusRegistrationConfigurator
        where TBus : class, IBus
    {
        /// <summary>
        /// Add bus rider
        /// </summary>
        /// <param name="configure"></param>
        void AddRider(Action<IRiderRegistrationConfigurator<TBus>> configure);
    }
}
