namespace MassTransit
{
    public interface StopSupervisorContext :
        StopContext
    {
        /// <summary>
        /// The agents available when the Stop was initiated
        /// </summary>
        IAgent[] Agents { get; }
    }
}
