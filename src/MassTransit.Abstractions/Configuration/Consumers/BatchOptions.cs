namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using Configuration;


    /// <summary>
    /// Batch options are applied to a <see cref="Batch{T}" /> consumer to configure
    /// the size and time limits for each batch.
    /// </summary>
    public class BatchOptions :
        IOptions,
        IConfigureReceiveEndpoint,
        ISpecification
    {
        public BatchOptions()
        {
            ConcurrencyLimit = 1;
            MessageLimit = 10;
            TimeLimit = TimeSpan.FromSeconds(1);
        }

        /// <summary>
        /// The maximum number of messages in a single batch
        /// </summary>
        public int MessageLimit { get; set; }

        /// <summary>
        /// The number of batches which can be executed concurrently
        /// </summary>
        public int ConcurrencyLimit { get; set; }

        /// <summary>
        /// The maximum time to wait before delivering a partial batch
        /// </summary>
        public TimeSpan TimeLimit { get; set; }

        /// <summary>
        /// The property to group by
        /// </summary>
        public object? GroupKeyProvider { get; private set; }

        public void Configure(string name, IReceiveEndpointConfigurator configurator)
        {
            var messageCapacity = ConcurrencyLimit * MessageLimit;

            configurator.PrefetchCount = Math.Max(messageCapacity, configurator.PrefetchCount);

            if (configurator.ConcurrentMessageLimit < messageCapacity)
                configurator.ConcurrentMessageLimit = messageCapacity;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (TimeLimit <= TimeSpan.Zero)
                yield return this.Failure("Batch", "TimeLimit", "Must be > TimeSpan.Zero");
            if (MessageLimit <= 0)
                yield return this.Failure("Batch", "MessageLimit", "Must be > 0");
            if (ConcurrencyLimit <= 0)
                yield return this.Failure("Batch", "ConcurrencyLimit", "Must be > 0");
        }

        /// <summary>
        /// Sets the maximum number of messages in a single batch
        /// </summary>
        /// <param name="limit">The message limit</param>
        /// <returns></returns>
        public BatchOptions SetMessageLimit(int limit)
        {
            MessageLimit = limit;
            return this;
        }

        /// <summary>
        /// Sets the number of batches which can be executed concurrently
        /// </summary>
        /// <param name="limit">The message limit</param>
        public BatchOptions SetConcurrencyLimit(int limit)
        {
            ConcurrencyLimit = limit;
            return this;
        }

        /// <summary>
        /// Sets the maximum time to wait before delivering a partial batch
        /// </summary>
        /// <param name="limit">The message limit</param>
        public BatchOptions SetTimeLimit(TimeSpan limit)
        {
            TimeLimit = limit;
            return this;
        }

        /// <summary>
        /// Sets the maximum time to wait before delivering a partial batch
        /// </summary>
        public BatchOptions SetTimeLimit(int? ms = default, int? s = default, int? m = default, int? h = default, int? d = default)
        {
            var timeSpan = new TimeSpan(d ?? 0, h ?? 0, m ?? 0, s ?? 0, ms ?? 0);
            if (timeSpan <= TimeSpan.Zero)
                throw new ArgumentException("The timeout must be > 0");

            TimeLimit = timeSpan;
            return this;
        }

        public BatchOptions GroupBy<T, TProperty>(Func<ConsumeContext<T>, TProperty?> provider)
            where T : class
            where TProperty : struct
        {
            GroupKeyProvider = new ValueTypeGroupKeyProvider<T, TProperty>(provider);

            return this;
        }

        public BatchOptions GroupBy<T, TProperty>(Func<ConsumeContext<T>, TProperty> provider)
            where T : class
            where TProperty : class
        {
            GroupKeyProvider = new GroupKeyProvider<T, TProperty>(provider);

            return this;
        }
    }
}
