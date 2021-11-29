namespace MassTransit
{
    using System;
    using System.Threading.Tasks;
    using Contracts;
    using Middleware;


    public static class CommandExtensions
    {
        public static Task SendCommand<T>(this IPipe<CommandContext> pipe, T command)
            where T : class
        {
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var context = new SendCommandContext<T>(command);

            return pipe.Send(context);
        }


        class SendCommandContext<T> :
            BasePipeContext,
            CommandContext<T>
            where T : class
        {
            public SendCommandContext(T command)
            {
                Command = command;
                Timestamp = DateTime.UtcNow;
            }

            public DateTime Timestamp { get; }

            public T Command { get; }
        }
    }
}
