namespace MassTransit.Courier.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;
    using Contracts;


    [Serializable]
    public class RoutingSlipCompensateLog :
        CompensateLog
    {
    #pragma warning disable CS8618
        public RoutingSlipCompensateLog()
    #pragma warning restore CS8618
        {
        }

        public RoutingSlipCompensateLog(Guid executionId, Uri address, IDictionary<string, object> data)
        {
            ExecutionId = executionId;
            Address = address;
            Data = data;
        }

        [SuppressMessage("ReSharper", "ConstantNullCoalescingCondition")]
        public RoutingSlipCompensateLog(CompensateLog compensateLog)
        {
            if (compensateLog.Address == null)
                throw new SerializationException("An CompensateLog CompensateAddress is required");

            ExecutionId = compensateLog.ExecutionId;
            Address = compensateLog.Address;
            Data = compensateLog.Data ?? new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public Guid ExecutionId { get; set; }
        public Uri Address { get; set; }
        public IDictionary<string, object> Data { get; set; }
    }
}
