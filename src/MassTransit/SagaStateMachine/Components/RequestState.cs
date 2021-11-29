namespace MassTransit.Components
{
    using System;


    public class RequestState :
        SagaStateMachineInstance
    {
        public int CurrentState { get; set; }

        public Guid? ConversationId { get; set; }
        public Uri ResponseAddress { get; set; }
        public Uri FaultAddress { get; set; }
        public DateTime? ExpirationTime { get; set; }

        /// <summary>
        /// The correlationId of the original saga instance
        /// </summary>
        public Guid SagaCorrelationId { get; set; }

        /// <summary>
        /// The saga address where the request should be redelivered
        /// </summary>
        public Uri SagaAddress { get; set; }

        /// <summary>
        /// Same as RequestId from the original request
        /// </summary>
        public Guid CorrelationId { get; set; }
    }
}
