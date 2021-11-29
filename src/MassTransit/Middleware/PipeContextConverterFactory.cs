namespace MassTransit.Middleware
{
    using System;
    using System.Linq;
    using Contracts;
    using Internals;


    public class PipeContextConverterFactory :
        IPipeContextConverterFactory<PipeContext>
    {
        public IPipeContextConverter<PipeContext, TOutput> GetConverter<TOutput>()
            where TOutput : class, PipeContext
        {
            if (typeof(TOutput).HasInterface<CommandContext>())
            {
                var innerType = typeof(TOutput).GetClosingArguments(typeof(CommandContext<>)).Single();

                return (IPipeContextConverter<PipeContext, TOutput>)Activator.CreateInstance(typeof(CommandContextConverter<>).MakeGenericType(innerType));
            }

            if (typeof(TOutput).HasInterface<EventContext>())
            {
                var innerType = typeof(TOutput).GetClosingArguments(typeof(EventContext<>)).Single();

                return (IPipeContextConverter<PipeContext, TOutput>)Activator.CreateInstance(typeof(EventContextConverter<>).MakeGenericType(innerType));
            }

            throw new ArgumentException($"The output type is not supported: {TypeCache<TOutput>.ShortName}", nameof(TOutput));
        }


        class CommandContextConverter<T> :
            IPipeContextConverter<PipeContext, CommandContext<T>>
            where T : class
        {
            bool IPipeContextConverter<PipeContext, CommandContext<T>>.TryConvert(PipeContext input, out CommandContext<T> output)
            {
                if (input is CommandContext<T> commandContext)
                {
                    output = commandContext;
                    return true;
                }

                output = null;
                return false;
            }
        }


        class EventContextConverter<T> :
            IPipeContextConverter<PipeContext, EventContext<T>>
            where T : class
        {
            bool IPipeContextConverter<PipeContext, EventContext<T>>.TryConvert(PipeContext input, out EventContext<T> output)
            {
                if (input is EventContext<T> eventContext)
                {
                    output = eventContext;
                    return true;
                }

                output = null;
                return false;
            }
        }
    }
}
