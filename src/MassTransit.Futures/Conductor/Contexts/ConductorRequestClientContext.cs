namespace MassTransit.Conductor.Contexts
{
    using System;


    public class ConductorRequestClientContext :
        RequestClientContext
    {
        public ConductorRequestClientContext(Guid clientId, Guid requestId)
        {
            ClientId = clientId;
            RequestId = requestId;
        }

        public Guid ClientId { get; }
        public Guid RequestId { get; }
    }
}