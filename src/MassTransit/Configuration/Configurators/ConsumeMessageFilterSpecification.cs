namespace MassTransit.Configurators
{
    using System;
    using System.Linq;
    using Util.Scanning;


    public class ConsumeMessageFilterSpecification :
        IMessageFilterConfigurator
    {
        public ConsumeMessageFilterSpecification()
        {
            Filter = new CompositeFilter<ConsumeContext>();
        }

        public CompositeFilter<ConsumeContext> Filter { get; }

        void IMessageFilterConfigurator.Include(params Type[] messageTypes)
        {
            Filter.Includes += message => Match(message, messageTypes);
        }

        void IMessageFilterConfigurator.Include<T>()
        {
            Filter.Includes += message => Match(message, typeof(T));
        }

        void IMessageFilterConfigurator.Include<T>(Func<T, bool> filter)
        {
            Filter.Includes += message => Match(message, filter);
        }

        void IMessageFilterConfigurator.Exclude(params Type[] messageTypes)
        {
            Filter.Excludes += message => Match(message, messageTypes);
        }

        void IMessageFilterConfigurator.Exclude<T>()
        {
            Filter.Excludes += message => Match(message, typeof(T));
        }

        void IMessageFilterConfigurator.Exclude<T>(Func<T, bool> filter)
        {
            Filter.Excludes += message => Match(message, filter);
        }

        static bool Match(ConsumeContext context, params Type[] messageTypes)
        {
            return messageTypes.Any(context.HasMessageType);
        }

        static bool Match<T>(ConsumeContext context, Func<T, bool> filter)
            where T : class
        {
            return context.TryGetMessage(out ConsumeContext<T> consumeContext) && filter(consumeContext.Message);
        }
    }
}
