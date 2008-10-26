namespace MassTransit.ServiceBus.Services.Timeout
{
    using System;
    using System.Collections.Generic;
    using Messages;

    public class RemoteTimeoutViewer :
        Consumes<ScheduleTimeout>.All,
        Consumes<TimeoutExpired>.All
    {
        private readonly IDictionary<Guid, ScheduleTimeout> _cache;

        public RemoteTimeoutViewer()
        {
            _cache = new Dictionary<Guid, ScheduleTimeout>();
        }

        public void Consume(ScheduleTimeout message)
        {
            _cache.Add(message.CorrelationId, message);
        }

        public void Consume(TimeoutExpired message)
        {
            _cache.Remove(message.CorrelationId);
        }

        public IList<ScheduleTimeout> List
        {
            get
            {
                return new List<ScheduleTimeout>(_cache.Values);
            }
        }
    }
}