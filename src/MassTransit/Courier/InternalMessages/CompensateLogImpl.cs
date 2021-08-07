namespace MassTransit.Courier.InternalMessages
{
    using System;
    using System.Collections.Generic;
    using Contracts;


    [Serializable]
    class CompensateLogImpl :
        CompensateLog
    {
        public CompensateLogImpl()
        {
        }

        public CompensateLogImpl(Guid executionId, Uri address, IDictionary<string, object> data)
        {
            ExecutionId = executionId;
            Address = address;
            Data = data;
        }

        public Guid ExecutionId { get; set; }
        public Uri Address { get; set; }
        public IDictionary<string, object> Data { get; set; }
    }
}
