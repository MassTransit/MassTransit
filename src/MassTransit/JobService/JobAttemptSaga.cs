namespace MassTransit
{
    using System;


    /// <summary>
    /// Each attempt to run a job is tracked by this state
    /// </summary>
    public class JobAttemptSaga :
        SagaStateMachineInstance,
        ISagaVersion
    {
        public int CurrentState { get; set; }

        public Guid JobId { get; set; }
        public int RetryAttempt { get; set; }
        public Uri ServiceAddress { get; set; }

        public Uri InstanceAddress { get; set; }

        public DateTime? Started { get; set; }
        public DateTime? Faulted { get; set; }

        public Guid? StatusCheckTokenId { get; set; }

        public byte[] RowVersion { get; set; }

        public int Version { get; set; }

        // AttemptId
        public Guid CorrelationId { get; set; }
    }
}
