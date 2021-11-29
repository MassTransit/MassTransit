namespace MassTransit
{
    using System.ComponentModel;
    using Configuration;


    public interface IPublishPipeConfigurator :
        IPipeConfigurator<PublishContext>,
        IPublishPipeSpecificationObserverConnector
    {
        /// <summary>
        /// Adds a type-specific pipe specification to the consume pipe
        /// </summary>
        /// <param name="specification"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        void AddPipeSpecification(IPipeSpecification<SendContext> specification);

        /// <summary>
        /// Adds a type-specific pipe specification to the consume pipe
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="specification"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        void AddPipeSpecification<T>(IPipeSpecification<SendContext<T>> specification)
            where T : class;

        /// <summary>
        /// Adds a type-specific pipe specification to the consume pipe
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="specification"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        void AddPipeSpecification<T>(IPipeSpecification<PublishContext<T>> specification)
            where T : class;
    }
}
