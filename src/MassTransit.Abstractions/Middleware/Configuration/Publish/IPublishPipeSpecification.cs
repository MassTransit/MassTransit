namespace MassTransit.Configuration
{
    public interface IPublishPipeSpecification :
        IPublishPipeSpecificationObserverConnector,
        ISpecification
    {
        /// <summary>
        /// Returns the specification for the message type
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <returns></returns>
        IMessagePublishPipeSpecification<T> GetMessageSpecification<T>()
            where T : class;
    }
}
