namespace MassTransit.Turnout.Components.StateMachines
{
    using System;
    using Automatonymous;


    /// <summary>
    /// Individual turnout jobs are tracked by this state
    /// </summary>
    public class TurnoutJobState :
        SagaStateMachineInstance
    {
        public int CurrentState { get; set; }

        public DateTime? Submitted { get; set; }
        public Uri ServiceAddress { get; set; }
        public TimeSpan JobTimeout { get; set; }
        public string JobJson { get; set; }
        public Guid JobTypeId { get; set; }

        public Guid AttemptId { get; set; }

        public DateTime? Started { get; set; }

        public DateTime? Completed { get; set; }
        public TimeSpan? Duration { get; set; }

        public DateTime? Faulted { get; set; }
        public string Reason { get; set; }

        public Guid? AttemptJobRequestId { get; set; }
        public Guid? AllocateJobSlotRequestId { get; set; }
        public Guid? JobSlotWaitToken { get; set; }

        public Guid CorrelationId { get; set; }
    }
}
