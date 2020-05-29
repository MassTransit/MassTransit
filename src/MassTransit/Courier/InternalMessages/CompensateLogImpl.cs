namespace MassTransit.Courier.InternalMessages
{
    using System;
    using System.Collections.Generic;
    using Contracts;


    class CompensateLogImpl :
        CompensateLog
    {
        public CompensateLogImpl(Guid executionId, Uri address,
            IDictionary<string, object> data)
        {
            ExecutionId = executionId;
            Address = address;
            Data = data;
        }

        public Guid ExecutionId { get; private set; }
        public Uri Address { get; private set; }
        public IDictionary<string, object> Data { get; private set; }
    }
}
