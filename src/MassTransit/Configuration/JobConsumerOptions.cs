namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using Configuration;


    public class JobConsumerOptions :
        IOptions,
        ISpecification
    {
        public JobConsumerOptions()
        {
            HeartbeatInterval = TimeSpan.FromMinutes(1);
        }

        public TimeSpan HeartbeatInterval { get; set; }

        IEnumerable<ValidationResult> ISpecification.Validate()
        {
            if (HeartbeatInterval <= TimeSpan.Zero)
                yield return this.Failure("JobConsumerOptions", "HeartbeatInterval", "Must be > 0");
        }

        public JobConsumerOptions SetHeartbeatInterval(int? d = null, int? h = null, int? m = null, int? s = null, int? ms = null)
        {
            var value = new TimeSpan(d ?? 0, h ?? 0, m ?? 0, s ?? 0, ms ?? 0);

            HeartbeatInterval = value;

            return this;
        }

        public JobConsumerOptions SetHeartbeatInterval(TimeSpan interval)
        {
            HeartbeatInterval = interval;

            return this;
        }
    }
}
