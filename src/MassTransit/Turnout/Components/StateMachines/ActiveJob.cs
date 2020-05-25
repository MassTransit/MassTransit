namespace MassTransit.Turnout.Components.StateMachines
{
    using System;


    /// <summary>
    /// Active Jobs are allocated a concurrency slot, and are valid until the deadline is reached, after
    /// which they may be automatically released.
    /// </summary>
    public class ActiveJob
    {
        public Guid JobId { get; set; }

        public DateTime Deadline { get; set; }
    }
}
