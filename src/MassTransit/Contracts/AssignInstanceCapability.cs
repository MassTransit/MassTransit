namespace MassTransit.Contracts
{
    using System;


    /// <summary>
    /// Supports node assignment by sending an Assign request to the address and receiving a NodeInfo in return
    /// </summary>
    public interface AssignInstanceCapability :
        ServiceCapability
    {
        /// <summary>
        /// The address where the Assign request should be sent (could be the same as the service address)
        /// </summary>
        Uri Address { get; }

        AssignInstanceCapabilityOptions Options { get; }
    }
}
