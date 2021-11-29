namespace MassTransit.Courier.Results
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts;


    class CompensatedCompensationResult<TLog> :
        CompensationResult
        where TLog : class
    {
        readonly CompensateContext<TLog> _compensateContext;
        readonly CompensateLog _compensateLog;
        readonly TimeSpan _duration;
        readonly IRoutingSlipEventPublisher _publisher;
        readonly RoutingSlip _routingSlip;

        public CompensatedCompensationResult(CompensateContext<TLog> compensateContext, IRoutingSlipEventPublisher publisher, CompensateLog compensateLog,
            RoutingSlip routingSlip)
        {
            _compensateContext = compensateContext;
            _publisher = publisher;
            _compensateLog = compensateLog;
            _routingSlip = routingSlip;
            _duration = _compensateContext.Elapsed;
        }

        public async Task Evaluate()
        {
            var builder = CreateRoutingSlipBuilder(_routingSlip);

            Build(builder);

            var routingSlip = builder.Build();

            await _publisher.PublishRoutingSlipActivityCompensated(_compensateContext.ActivityName, _compensateContext.ExecutionId,
                _compensateContext.Timestamp, _duration, _routingSlip.Variables, _compensateLog.Data).ConfigureAwait(false);

            if (HasMoreCompensations(routingSlip))
            {
                var endpoint = await _compensateContext.GetSendEndpoint(routingSlip.GetNextCompensateAddress()).ConfigureAwait(false);

                await _compensateContext.Forward(endpoint, routingSlip).ConfigureAwait(false);
            }
            else
            {
                var faultedTimestamp = _compensateContext.Timestamp + _duration;
                var faultedDuration = faultedTimestamp - _routingSlip.CreateTimestamp;

                await _publisher.PublishRoutingSlipFaulted(faultedTimestamp, faultedDuration, _routingSlip.Variables,
                    _routingSlip.ActivityExceptions.ToArray()).ConfigureAwait(false);
            }
        }

        public bool IsFailed(out Exception exception)
        {
            exception = default;
            return false;
        }

        bool HasMoreCompensations(RoutingSlip routingSlip)
        {
            return routingSlip.CompensateLogs != null && routingSlip.CompensateLogs.Count > 0;
        }

        protected virtual void Build(RoutingSlipBuilder builder)
        {
        }

        protected virtual RoutingSlipBuilder CreateRoutingSlipBuilder(RoutingSlip routingSlip)
        {
            return new RoutingSlipBuilder(routingSlip, SkipLast(routingSlip.CompensateLogs));
        }

        static IEnumerable<T> SkipLast<T>(IEnumerable<T> source)
        {
            using IEnumerator<T> enumerator = source.GetEnumerator();

            if (enumerator.MoveNext())
            {
                var element = enumerator.Current;

                while (enumerator.MoveNext())
                {
                    yield return element;
                    element = enumerator.Current;
                }
            }
        }
    }
}
