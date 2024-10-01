namespace MassTransit.Contracts.JobService;

public static class JobCancellationReasons
{
    public static readonly string Shutdown = "Job Service Shutdown";
    public static readonly string CancellationRequested = "Cancellation Requested";
    public static readonly string ConsumerInitiated = "Consumer Initiated";
}
