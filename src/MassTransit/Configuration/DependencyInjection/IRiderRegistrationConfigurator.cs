namespace MassTransit
{
    using Configuration;
    using DependencyInjection;
    using Transports;


    public interface IRiderRegistrationConfigurator :
        IRegistrationConfigurator
    {
        IContainerRegistrar Registrar { get; }

        /// <summary>
        /// Add the rider to the container, configured properly
        /// </summary>
        /// <param name="riderFactory"></param>
        void SetRiderFactory<TRider>(IRegistrationRiderFactory<TRider> riderFactory)
            where TRider : class, IRider;
    }


    public interface IRiderRegistrationConfigurator<in TBus> :
        IRiderRegistrationConfigurator
        where TBus : class, IBus
    {
    }
}
