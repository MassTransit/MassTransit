namespace MassTransit.Registration
{
    using Riders;


    public interface IRiderConfigurator :
        IRegistrationConfigurator
    {
        IContainerRegistrar Registrar { get; }
    }


    public interface IRiderConfigurator<out TContainerContext> :
        IRiderConfigurator
        where TContainerContext : class
    {
        /// <summary>
        /// Add the rider to the container, configured properly
        /// </summary>
        /// <param name="riderFactory"></param>
        void SetRiderFactory<TRider>(IRegistrationRiderFactory<TContainerContext, TRider> riderFactory)
            where TRider : class, IRider;
    }
}
