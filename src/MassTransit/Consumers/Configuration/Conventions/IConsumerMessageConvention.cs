namespace MassTransit.Configuration
{
    using System.Collections.Generic;


    /// <summary>
    /// A convention that returns connectors for message types that are defined in the consumer
    /// type.
    /// </summary>
    public interface IConsumerMessageConvention
    {
        /// <summary>
        /// Returns the message types handled by the consumer class
        /// </summary>
        /// <returns></returns>
        IEnumerable<IMessageInterfaceType> GetMessageTypes();
    }
}
