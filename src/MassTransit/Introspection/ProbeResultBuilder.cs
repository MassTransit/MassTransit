namespace MassTransit.Introspection
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Metadata;


    public class ProbeResultBuilder :
        ScopeProbeContext,
        IProbeResultBuilder
    {
        readonly Guid _probeId;
        readonly Guid _resultId;
        readonly DateTime _startTimestamp;

        public ProbeResultBuilder(Guid probeId, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _probeId = probeId;

            _resultId = Guid.NewGuid();
            _startTimestamp = DateTime.UtcNow;
        }

        public new ProbeResult Build()
        {
            var duration = DateTime.UtcNow - _startTimestamp;

            return new Result(_probeId, _resultId, _startTimestamp, duration, HostMetadataCache.Host, base.Build());
        }


        class Result :
            ProbeResult
        {
            public Result(Guid probeId, Guid resultId, DateTime startTimestamp, TimeSpan duration, HostInfo host, IDictionary<string, object> results)
            {
                ProbeId = probeId;
                ResultId = resultId;
                StartTimestamp = startTimestamp;
                Duration = duration;
                Host = host;
                Results = results;
            }

            public Guid ResultId { get; }
            public Guid ProbeId { get; }
            public DateTime StartTimestamp { get; }
            public TimeSpan Duration { get; }
            public HostInfo Host { get; }
            public IDictionary<string, object> Results { get; }
        }
    }
}
