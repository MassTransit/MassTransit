namespace MassTransit.Configuration
{
    using System.Collections.Generic;
    using System.Linq;


    public static class ConsumerConventionCache
    {
        static ConsumerConventionCache()
        {
            ConsumerConvention.Register<AsyncConsumerConvention>();
            ConsumerConvention.Register<BatchConsumerConvention>();
            ConsumerConvention.Register<JobConsumerConvention>();
        }

        public static bool TryAdd<T>(T convention)
            where T : IConsumerConvention
        {
            if (Cached.Registered.Any(x => x.GetType() == convention.GetType()))
                return false;

            Cached.Registered.Add(convention);

            return true;
        }

        public static bool Remove<T>()
            where T : IConsumerConvention
        {
            for (var i = 0; i < Cached.Registered.Count; i++)
            {
                if (Cached.Registered[i] is T)
                {
                    Cached.Registered.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the conventions registered for identifying message consumer types
        /// </summary>
        /// <typeparam name="T">The consumer type</typeparam>
        /// <returns></returns>
        public static IEnumerable<IConsumerMessageConvention> GetConventions<T>()
            where T : class
        {
            return Cached.Registered.Select(convention => convention.GetConsumerMessageConvention<T>());
        }


        static class Cached
        {
            internal static readonly IList<IConsumerConvention> Registered = new List<IConsumerConvention>();
        }
    }
}
