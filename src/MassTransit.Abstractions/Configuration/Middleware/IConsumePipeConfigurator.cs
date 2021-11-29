namespace MassTransit
{
    using System.ComponentModel;
    using Configuration;


    public interface IConsumePipeConfigurator :
        IPipeConfigurator<ConsumeContext>,
        IConsumerConfigurationObserverConnector,
        ISagaConfigurationObserverConnector,
        IHandlerConfigurationObserverConnector,
        IActivityConfigurationObserverConnector,
        IConsumerConfigurationObserver,
        ISagaConfigurationObserver,
        IHandlerConfigurationObserver,
        IActivityConfigurationObserver
    {
        /// <summary>
        /// If set to false, the transport will only be started when a connection is made to the consume pipe.
        /// </summary>
        bool AutoStart { set; }

        /// <summary>
        /// Adds a type-specific pipe specification to the consume pipe
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="specification"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        void AddPipeSpecification<T>(IPipeSpecification<ConsumeContext<T>> specification)
            where T : class;

        /// <summary>
        /// Adds a pipe specification prior to the message type router so that a single
        /// instance is used for all message types
        /// </summary>
        /// <param name="specification"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        void AddPrePipeSpecification(IPipeSpecification<ConsumeContext> specification);
    }
}
