namespace MassTransit.SignalR.Utils
{
    using Microsoft.AspNetCore.SignalR;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    public static class HubConsumersCache
    {
        private static IEnumerable<Type> ConsumerTypes { get; } = from t in typeof(MassTransitDependencyInjectionExtensions).Assembly.GetTypes()
                                                                  where typeof(IConsumer).IsAssignableFrom(t)
                                                                  select t;

        public static IEnumerable<Type> GetOrAdd<THub>()
            where THub : Hub
        {
            return GetOrAdd(typeof(THub));
        }

        public static IEnumerable<Type> GetOrAdd(Type hubType)
        {
            if (!typeof(Hub).IsAssignableFrom(hubType)) throw new ArgumentException("hubType must implement a SignalR Hub");

            var consumers = new List<Type>();

            foreach (var consumer in ConsumerTypes)
            {
                var consumerType = consumer.MakeGenericType(hubType);
                consumers.Add(consumerType);
            }

            return Cached.Instance.GetOrAdd(hubType, _ => consumers);
        }

        static class Cached
        {
            internal static readonly ConcurrentDictionary<Type, IEnumerable<Type>> Instance = new ConcurrentDictionary<Type, IEnumerable<Type>>();
        }
    }
}
