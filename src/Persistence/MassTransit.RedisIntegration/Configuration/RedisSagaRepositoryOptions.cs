namespace MassTransit
{
    using System;


    public class RedisSagaRepositoryOptions<TSaga>
        where TSaga : class, ISaga
    {
        public RedisSagaRepositoryOptions(ConcurrencyMode concurrencyMode, TimeSpan? lockTimeout, string lockSuffix, string keyPrefix,
            SelectDatabase databaseSelector, TimeSpan? expiry)
        {
            ConcurrencyMode = concurrencyMode;

            LockTimeout = lockTimeout ?? TimeSpan.FromSeconds(30);

            LockSuffix = string.IsNullOrEmpty(lockSuffix) ? "_lock" : lockSuffix;

            KeyPrefix = string.IsNullOrWhiteSpace(keyPrefix) ? null : keyPrefix.EndsWith(":") ? keyPrefix : $"{keyPrefix}:";

            RetryPolicy = Retry.Exponential(10, TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(10), TimeSpan.FromMilliseconds(918));

            DatabaseSelector = databaseSelector;

            Expiry = expiry;
        }

        public IRetryPolicy RetryPolicy { get; }

        public string KeyPrefix { get; }
        public TimeSpan LockTimeout { get; }
        public string LockSuffix { get; }
        public ConcurrencyMode ConcurrencyMode { get; }
        public SelectDatabase DatabaseSelector { get; }
        public TimeSpan? Expiry { get; }

        public string FormatSagaKey(Guid correlationId)
        {
            return KeyPrefix != null ? $"{KeyPrefix}{correlationId}" : correlationId.ToString();
        }

        public string FormatLockKey(Guid correlationId)
        {
            return LockSuffix != null
                ? KeyPrefix != null ? $"{KeyPrefix}{correlationId}{LockSuffix}" : $"{correlationId}{LockSuffix}"
                : FormatSagaKey(correlationId);
        }
    }
}
