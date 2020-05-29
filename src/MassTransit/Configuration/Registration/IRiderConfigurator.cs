namespace MassTransit.Registration
{
    public interface IRiderConfigurator<out TContainerContext> :
        IRegistrationConfigurator
        where TContainerContext : class
    {
        /// <summary>
        /// Add the rider to the container, configured properly
        /// </summary>
        /// <param name="riderFactory"></param>
        void SetRiderFactory<T>(T riderFactory)
            where T : IRegistrationRiderFactory<TContainerContext>;
    }
}
