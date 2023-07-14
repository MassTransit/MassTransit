namespace MassTransit.Configuration
{
    using System;


    /// <summary>
    /// Settings used by the job service sagas
    /// </summary>
    public interface JobSagaSettings
    {
        Uri JobAttemptSagaEndpointAddress { get; }
        Uri JobSagaEndpointAddress { get; }
        Uri JobTypeSagaEndpointAddress { get; }

        TimeSpan StatusCheckInterval { get; }

        int SuspectJobRetryCount { get; }
        TimeSpan? SuspectJobRetryDelay { get; }

        TimeSpan SlotWaitTime { get; }

        TimeSpan HeartbeatTimeout { get; }

        bool FinalizeCompleted { get; }
    }
}
