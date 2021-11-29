namespace MassTransit.Configuration
{
    using System;
    using System.Linq;


    public class ConsumeMessageFilterConfigurator :
        IMessageFilterConfigurator
    {
        public ConsumeMessageFilterConfigurator()
        {
            Filter = new CompositeFilter<ConsumeContext>();
        }

        public CompositeFilter<ConsumeContext> Filter { get; }

        public void Include(params Type[] messageTypes)
        {
            Filter.Includes += message => Match(message, messageTypes);
        }

        public void Include<T>()
            where T : class
        {
            Filter.Includes += message => Match<T>(message);
        }

        public void Include<T>(Func<T, bool> filter)
            where T : class
        {
            Filter.Includes += message => Match(message, filter);
        }

        public void Exclude(params Type[] messageTypes)
        {
            Filter.Excludes += message => Match(message, messageTypes);
        }

        public void Exclude<T>()
            where T : class
        {
            Filter.Excludes += message => Match<T>(message);
        }

        public void Exclude<T>(Func<T, bool> filter)
            where T : class
        {
            Filter.Excludes += message => Match(message, filter);
        }

        static bool Match(ConsumeContext context, params Type[] messageTypes)
        {
            return messageTypes.Any(type => context.HasMessageType(type));
        }

        static bool Match<T>(ConsumeContext context)
            where T : class
        {
            return context.TryGetMessage(out ConsumeContext<T> _);
        }

        static bool Match<T>(ConsumeContext context, Func<T, bool> filter)
            where T : class
        {
            return context.TryGetMessage(out ConsumeContext<T> consumeContext) && filter(consumeContext.Message);
        }
    }
}
