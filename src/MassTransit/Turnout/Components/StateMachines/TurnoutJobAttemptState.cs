namespace MassTransit.Turnout.Components.StateMachines
{
    using System;
    using Automatonymous;


    /// <summary>
    /// Each attempt to run a job is tracked by this state
    /// </summary>
    public class TurnoutJobAttemptState :
        SagaStateMachineInstance
    {
        public int CurrentState { get; set; }

        public Guid JobId { get; set; }
        public int RetryAttempt { get; set; }
        public Uri ServiceAddress { get; set; }

        public Uri InstanceAddress { get; set; }

        public DateTime? Started { get; set; }
        public DateTime? Faulted { get; set; }

        public Guid? StatusCheckTokenId { get; set; }

        // AttemptId
        public Guid CorrelationId { get; set; }
    }
}
