namespace MassTransit
{
    using System;
    using Registration;


    /// <summary>
    /// Configures the container registration, and supports creation of a bus or a mediator.
    /// </summary>
    public interface IBusRegistrationConfigurator :
        IRegistrationConfigurator
    {
        /// <summary>
        /// This method is being deprecated. Use the transport-specific UsingRabbitMq, UsingActiveMq, etc. methods instead.
        /// </summary>
        /// <param name="busFactory"></param>
        void AddBus(Func<IBusRegistrationContext, IBusControl> busFactory);

        /// <summary>
        /// Sets the bus factory. This is used by the transport extension methods (such as UsingRabbitMq, Using ActiveMq, etc.) to
        /// specify the bus factory. The extension method approach is preferred (since v7) over the AddBus method.
        /// </summary>
        /// <param name="busFactory"></param>
        /// <typeparam name="T"></typeparam>
        void SetBusFactory<T>(T busFactory)
            where T : IRegistrationBusFactory;

        /// <summary>
        /// Add bus rider
        /// </summary>
        /// <param name="configure"></param>
        void AddRider(Action<IRiderRegistrationConfigurator> configure);
    }
}
