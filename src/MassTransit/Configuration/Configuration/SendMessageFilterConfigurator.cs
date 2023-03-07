namespace MassTransit.Configuration
{
    using System;
    using System.Linq;
    using Internals;


    public class SendMessageFilterConfigurator :
        IMessageFilterConfigurator
    {
        public SendMessageFilterConfigurator()
        {
            Filter = new CompositeFilter<SendContext>();
        }

        public CompositeFilter<SendContext> Filter { get; }

        void IMessageTypeFilterConfigurator.Include(params Type[] messageTypes)
        {
            Filter.Includes += message => Match(message, messageTypes);
        }

        void IMessageTypeFilterConfigurator.Include(Func<Type, bool> filter)
        {
            Filter.Includes += context => context.GetType().ClosesType(typeof(SendContext<>), out Type[] types) && filter(types[0]);
        }

        void IMessageTypeFilterConfigurator.Include<T>()
        {
            Filter.Includes += message => Match<T>(message);
        }

        void IMessageFilterConfigurator.Include<T>(Func<T, bool> filter)
        {
            Filter.Includes += message => Match(message, filter);
        }

        void IMessageTypeFilterConfigurator.Exclude(params Type[] messageTypes)
        {
            Filter.Excludes += message => Match(message, messageTypes);
        }

        void IMessageTypeFilterConfigurator.Exclude(Func<Type, bool> filter)
        {
            Filter.Excludes += context => context.GetType().ClosesType(typeof(SendContext<>), out Type[] types) && filter(types[0]);
        }

        void IMessageTypeFilterConfigurator.Exclude<T>()
        {
            Filter.Excludes += message => Match<T>(message);
        }

        void IMessageFilterConfigurator.Exclude<T>(Func<T, bool> filter)
        {
            Filter.Excludes += message => Match(message, filter);
        }

        static bool Match(SendContext context, params Type[] messageTypes)
        {
            return messageTypes.Any(x => typeof(SendContext<>).MakeGenericType(x).IsInstanceOfType(context));
        }

        static bool Match<T>(SendContext context)
            where T : class
        {
            return context is SendContext<T>;
        }

        static bool Match<T>(SendContext context, Func<T, bool> filter)
            where T : class
        {
            var sendContext = context as SendContext<T>;

            return sendContext != null && filter(sendContext.Message);
        }
    }
}
