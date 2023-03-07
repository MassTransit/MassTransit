namespace MassTransit.Configuration
{
    using System;
    using System.Linq;


    public class MessageTypeFilterConfigurator :
        IMessageTypeFilterConfigurator
    {
        public MessageTypeFilterConfigurator()
        {
            Filter = new CompositeFilter<Type>();
        }

        public CompositeFilter<Type> Filter { get; }

        public void Include(params Type[] messageTypes)
        {
            Filter.Includes += type => Match(type, messageTypes);
        }

        public void Include(Func<Type, bool> filter)
        {
            Filter.Includes += type => filter(type);
        }

        public void Include<T>()
            where T : class
        {
            Filter.Includes += type => Match<T>(type);
        }

        public void Exclude(params Type[] messageTypes)
        {
            Filter.Excludes += type => Match(type, messageTypes);
        }

        public void Exclude(Func<Type, bool> filter)
        {
            Filter.Excludes += type => filter(type);
        }

        public void Exclude<T>()
            where T : class
        {
            Filter.Excludes += type => Match<T>(type);
        }

        static bool Match(Type type, params Type[] messageTypes)
        {
            return messageTypes.Any(x => x == type);
        }

        static bool Match<T>(Type type)
            where T : class
        {
            return type == typeof(T);
        }
    }
}
