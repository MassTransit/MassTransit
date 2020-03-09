namespace MassTransit.Contracts
{
    using System;


    /// <summary>
    /// A service instance
    /// </summary>
    public interface InstanceInfo
    {
        /// <summary>
        /// Uniquely identifies the instance hosting the service endpoint
        /// </summary>
        Guid InstanceId { get; }

        /// <summary>
        /// When the instance started, if it has started
        /// </summary>
        DateTime? Started { get; }
    }
}
