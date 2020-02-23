namespace MassTransit.Configurators
{
    using System;
    using System.Linq;
    using Util.Scanning;


    public class SendMessageFilterSpecification :
        IMessageFilterConfigurator
    {
        readonly CompositeFilter<SendContext> _filter;

        public SendMessageFilterSpecification()
        {
            _filter = new CompositeFilter<SendContext>();
        }

        public CompositeFilter<SendContext> Filter => _filter;

        void IMessageFilterConfigurator.Include(params Type[] messageTypes) =>
            _filter.Includes += message => Match(message, messageTypes);

        void IMessageFilterConfigurator.Include<T>() =>
            _filter.Includes += message => Match(message, typeof(T));

        void IMessageFilterConfigurator.Include<T>(Func<T, bool> filter) =>
            _filter.Includes += message => Match(message, filter);

        void IMessageFilterConfigurator.Exclude(params Type[] messageTypes) =>
            _filter.Excludes += message => Match(message, messageTypes);

        void IMessageFilterConfigurator.Exclude<T>() =>
            _filter.Excludes += message => Match(message, typeof(T));

        void IMessageFilterConfigurator.Exclude<T>(Func<T, bool> filter) =>
            _filter.Excludes += message => Match(message, filter);

        static bool Match(SendContext context, params Type[] messageTypes)
        {
            return messageTypes.Any(x => typeof(SendContext<>).MakeGenericType(x).IsInstanceOfType(context));
        }

        static bool Match<T>(SendContext context, Func<T, bool> filter)
            where T : class
        {
            var sendContext = context as SendContext<T>;

            return sendContext != null && filter(sendContext.Message);
        }
    }
}
