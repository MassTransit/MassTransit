namespace MassTransit
{
    using System;
    using ConsumeConfigurators;


    /// <summary>
    /// Batch options are applied to a <see cref="Batch{T}"/> consumer to configure
    /// the size and time limits for each batch.
    /// </summary>
    public class BatchOptions :
        IOptions
    {
        public BatchOptions()
        {
            MessageLimit = 10;
            TimeLimit = TimeSpan.FromSeconds(1);
        }

        public BatchOptions SetMessageLimit(int limit)
        {
            MessageLimit = limit;
            return this;
        }

        public BatchOptions SetTimeLimit(TimeSpan limit)
        {
            TimeLimit = limit;
            return this;
        }

        public BatchOptions SetTimeLimit(int? ms = default, int? s = default, int? m = default, int? h = default, int? d = default)
        {
            var timeSpan = new TimeSpan(d ?? 0, h ?? 0, m ?? 0, s ?? 0, ms ?? 0);
            if (timeSpan <= TimeSpan.Zero)
                throw new ArgumentException("The timeout must be > 0");

            TimeLimit = timeSpan;
            return this;
        }

        public int MessageLimit { get; private set; }
        public TimeSpan TimeLimit { get; private set; }
    }
}
