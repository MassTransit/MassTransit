namespace MassTransit
{
    using System;
    using Configuration;


    /// <summary>
    /// Batch options are applied to a <see cref="Batch{T}" /> consumer to configure
    /// the size and time limits for each batch.
    /// </summary>
    public class BatchOptions :
        IOptions
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
        /// The condition upon which to group messages in different batches
        /// </summary>
        public Func<ConsumeContext, object> GroupingExpression { get; set; }

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

        /// <summary>
        /// Sets the condition upon which to group messages in different batches
        /// </summary>
        /// <param name="groupingExpression">Batches grouping expression</param>
        public BatchOptions GroupBy(Func<ConsumeContext, object> groupingExpression)
        {
            GroupingExpression = groupingExpression;
            return this;
        }
    }
}
