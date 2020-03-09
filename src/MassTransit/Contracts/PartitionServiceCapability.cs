namespace MassTransit.Contracts
{
    using System;


    /// <summary>
    /// The instance supports client message partitioning,
    /// </summary>
    public interface PartitionServiceCapability :
        ServiceCapability
    {
        /// <summary>
        /// The node's direct address for messages
        /// </summary>
        Uri Address { get; }

        /// <summary>
        /// The message properties used to generate the partition key
        /// </summary>
        PropertyInfo[] Properties { get; }
    }
}
