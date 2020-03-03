namespace MassTransit.Conductor.Configuration
{
    public interface IServiceInstanceConfigurator<out TEndpointConfigurator> :
        IReceiveConfigurator<TEndpointConfigurator>
        where TEndpointConfigurator : IReceiveEndpointConfigurator
    {
        IEndpointNameFormatter EndpointNameFormatter { get; }

        /// <summary>
        /// Adds client tracking, which keep track of each client instance using the service endpoint(s)
        /// </summary>
        //        void EnableClientTracking();

        /// <summary>
        /// Adds the ability to assign a message series to a specific instance. Once assigned, the client
        /// will send the entire series of messages to the instance's endpoint.
        /// </summary>
        //      void EnableAssignment();

        /// <summary>
        /// Enables service client partitioning so that requests are distributed across available instances
        /// using a consistent hash of the specified message properties.
        /// </summary>
        //        void EnableClientPartitioning(Action<IPartitionConfigurator> configure = null);
    }
}
