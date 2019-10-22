namespace MassTransit.Contracts
{
    using System;


    public interface StateInfo
    {
        string Name { get; }

        /// <summary>
        /// The event types accepted in this state
        /// </summary>
        Uri[] EventTypes { get; }
    }
}
