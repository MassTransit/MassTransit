namespace MassTransit.SendPipeSpecifications
{
    using GreenPipes;


    public interface ISendPipeSpecification :
        ISendPipeSpecificationObserverConnector,
        ISpecification
    {
        /// <summary>
        /// Returns the specification for the message type
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <returns></returns>
        IMessageSendPipeSpecification<T> GetMessageSpecification<T>()
            where T : class;
    }
}
