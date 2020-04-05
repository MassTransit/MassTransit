namespace MassTransit.ConsumeConnectors
{
    using System.Collections.Generic;
    using System.Linq;


    public static class ConsumerConventionCache
    {
        static ConsumerConventionCache()
        {
            ConsumerConvention.Register<AsyncConsumerConvention>();
            ConsumerConvention.Register<LegacyConsumerConvention>();
            ConsumerConvention.Register<BatchConsumerConvention>();
        }

        public static void Add<T>(T convention)
            where T : IConsumerConvention
        {
            Cached.Registered.Add(convention);
        }

        public static bool Remove<T>()
            where T : IConsumerConvention
        {
            for (int i = 0; i < Cached.Registered.Count; i++)
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
