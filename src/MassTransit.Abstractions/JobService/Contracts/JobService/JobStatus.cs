namespace MassTransit.Contracts.JobService
{
    public enum JobStatus
    {
        Running,
        Faulted,
        Completed,
        Canceled
    }
}
