namespace MassTransit.JobService.Components
{
    /// <summary>
    /// The status of a job
    /// </summary>
    public enum JobStatus
    {
        Created,
        Running,
        Faulted,
        RanToCompletion,
        Canceled
    }
}
