namespace MassTransit.MongoDbIntegration.Courier.Events
{
    using System;
    using Documents;


    public abstract class RoutingSlipEventDocument
    {
        protected RoutingSlipEventDocument(DateTime timestamp, TimeSpan duration, HostInfo host = null)
        {
            Timestamp = timestamp;
            Duration = duration;

            if (host != null)
                Host = new HostDocument(host);
        }

        public DateTime Timestamp { get; private set; }
        public TimeSpan Duration { get; private set; }
        public HostDocument Host { get; private set; }
    }
}
