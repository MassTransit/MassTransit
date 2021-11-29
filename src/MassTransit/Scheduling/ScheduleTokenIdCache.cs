namespace MassTransit.Scheduling
{
    using System;
    using System.Threading;


    /// <summary>
    /// A cache of convention-based CorrelationId mappers, used unless overridden by some mystical force
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ScheduleTokenIdCache<T> :
        IScheduleTokenIdCache<T>
        where T : class
    {
        public delegate Guid? TokenIdSelector(T instance);


        readonly TokenIdSelector _selector;

        ScheduleTokenIdCache(TokenIdSelector selector)
        {
            _selector = selector;
        }

        ScheduleTokenIdCache()
        {
            _selector = x => default;
        }

        public bool TryGetTokenId(T message, out Guid tokenId)
        {
            Guid? result = _selector(message);
            if (result.HasValue)
            {
                tokenId = result.Value;
                return true;
            }

            tokenId = default;
            return false;
        }

        public static Guid GetTokenId(T message, Guid? defaultValue = default)
        {
            if (Cached.Metadata.Value.TryGetTokenId(message, out var tokenId))
                return tokenId;

            return defaultValue ?? NewId.NextGuid();
        }

        internal static void UseTokenId(TokenIdSelector tokenIdSelector)
        {
            if (Cached.Metadata.IsValueCreated)
                return;

            Cached.Metadata = new Lazy<IScheduleTokenIdCache<T>>(() => new ScheduleTokenIdCache<T>(tokenIdSelector));
        }


        static class Cached
        {
            internal static Lazy<IScheduleTokenIdCache<T>> Metadata = new Lazy<IScheduleTokenIdCache<T>>(
                () => new ScheduleTokenIdCache<T>(), LazyThreadSafetyMode.PublicationOnly);
        }
    }
}
