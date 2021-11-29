namespace MassTransit.Contracts
{
    using System;


    public interface CommandContext :
        PipeContext
    {
        /// <summary>
        /// The timestamp at which the command was sent
        /// </summary>
        DateTime Timestamp { get; }
    }


    public interface CommandContext<out T> :
        CommandContext
        where T : class
    {
        /// <summary>
        /// The command object
        /// </summary>
        T Command { get; }
    }
}
