namespace MassTransit
{
    /// <summary>
    /// The context associated with stopping an agent
    /// </summary>
    public interface StopContext :
        PipeContext
    {
        /// <summary>
        /// The reason for stopping
        /// </summary>
        string Reason { get; }
    }
}
