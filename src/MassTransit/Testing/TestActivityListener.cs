#nullable enable
namespace MassTransit.Testing
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Logging;
    using Util;


    public class TestActivityListener :
        IAsyncDisposable
    {
        static readonly ActivitySource _source = new ActivitySource("MassTransit.TestHarness");

        readonly string? _className;
        readonly bool _includeDetails;
        readonly ActivityListener _listener;
        readonly Activity? _testActivity;
        readonly ConcurrentDictionary<string, TraceInfo> _traces;
        readonly TextWriter _writer;

        public TestActivityListener(TextWriter writer, string? methodName, string? className, bool includeDetails)
        {
            _writer = writer;
            _className = className;
            _includeDetails = includeDetails;

            _traces = new ConcurrentDictionary<string, TraceInfo>();

            Activity.DefaultIdFormat = ActivityIdFormat.W3C;
            Activity.ForceDefaultIdFormat = true;

            _listener = new ActivityListener
            {
                ShouldListenTo = _ => true,
                Sample = Sample,
                ActivityStarted = ActivityStarted,
                ActivityStopped = ActivityStopped
            };

            ActivitySource.AddActivityListener(_listener);

            if (methodName != null)
                _testActivity = _source.StartActivity(methodName);
        }

        public async ValueTask DisposeAsync()
        {
            _testActivity?.Stop();
            _testActivity?.Dispose();

            _listener.Dispose();

            await GenerateOutput().ConfigureAwait(false);
        }

        async Task GenerateOutput()
        {
            var chart = new ChartTable(50);

            foreach (var trace in _traces.Values.OrderBy(x => x.StartTime))
            {
                var stack = new Stack<(int, SpanInfo)>();

                foreach (var topSpan in trace.Spans.Values.Where(x => string.IsNullOrWhiteSpace(x.ParentId)).OrderByDescending(x => x.StartTime))
                    stack.Push((0, topSpan));

                while (stack.Any())
                {
                    var (depth, span) = stack.Pop();

                    var sb = new StringBuilder(60);

                    for (var i = 1; i < depth; i++)
                    {
                        if (stack.Any(x => x.Item1 == i))
                            sb.Append("\x2502 ");
                        else
                            sb.Append("  ");
                    }

                    if (depth > 0)
                        sb.Append(stack.Any(x => x.Item1 == depth) ? "\x251d " : "\x2514 ");

                    sb.Append(span.OperationName);

                    var details = FormatDetailsColumn(span);

                    chart.Add(sb.ToString(), span.StartTime.LocalDateTime, span.Duration, details);

                    foreach (var childSpan in trace.Spans.Values.Where(x => x.ParentId == span.SpanId).OrderByDescending(x => x.StartTime))
                        stack.Push((depth + 1, childSpan));
                }
            }

            var table = _includeDetails
                ? TextTable.Create(chart.GetRows().Select(x => new
                {
                    x.Title,
                    x.Duration,
                    x.Timeline,
                    Details = x.GetColumn(0)
                }))
                : TextTable.Create(chart.GetRows());

            table
                .SetColumn(0, _className ?? "Operation Name")
                .SetColumn(1, "Duration", typeof(int))
                .SetRightNumberAlignment()
                .OutputTo(_writer)
                .Write();
        }

        static ActivitySamplingResult Sample(ref ActivityCreationOptions<ActivityContext> options)
        {
            return ActivitySamplingResult.AllDataAndRecorded;
        }

        void ActivityStarted(Activity activity)
        {
            GetSpan(activity);
        }

        void ActivityStopped(Activity activity)
        {
            var span = GetSpan(activity);

            span.Duration = activity.Duration;
        }

        SpanInfo GetSpan(Activity activity)
        {
            var traceId = activity.RootId ?? "";

            var trace = _traces.GetOrAdd(traceId, id => new TraceInfo { StartTime = activity.StartTimeUtc });

            var span = trace.Spans.GetOrAdd(activity.Id ?? "", id => new SpanInfo
            {
                SpanId = id,
                ParentId = activity.ParentId,
                StartTime = activity.StartTimeUtc,
                OperationName = activity.OperationName,
                Activity = activity,
            });

            if (activity.Duration > TimeSpan.Zero)
            {
                var traceDuration = activity.StartTimeUtc - trace.StartTime + activity.Duration;

                if (traceDuration > trace.Duration)
                    trace.Duration = traceDuration;
            }

            return span;
        }

        static string FormatDetailsColumn(SpanInfo span)
        {
            if (span.Activity?.GetTagItem(DiagnosticHeaders.SagaId) is string sagaId)
            {
                if (span.Activity?.GetTagItem(DiagnosticHeaders.BeginState) is string beginState
                    && span.Activity?.GetTagItem(DiagnosticHeaders.EndState) is string endState)
                    return $"{sagaId}: {beginState} -> {endState}";

                return $"{sagaId}";
            }

            if (span.Activity?.GetTagItem(DiagnosticHeaders.ConsumerType) is string consumerType)
            {
                if (span.Activity?.GetTagItem(DiagnosticHeaders.RequestId) is string requestId)
                    return $"{requestId}: {consumerType}";

                return $"{consumerType}";
            }

            return string.Empty;
        }


        class TraceInfo
        {
            public TraceInfo()
            {
                Spans = new ConcurrentDictionary<string, SpanInfo>();
            }

            public DateTimeOffset StartTime { get; set; }
            public TimeSpan Duration { get; set; }

            public ConcurrentDictionary<string, SpanInfo> Spans { get; set; }
        }


        class SpanInfo
        {
            public string? SpanId { get; set; }
            public DateTimeOffset StartTime { get; set; }
            public TimeSpan Duration { get; set; }
            public string? ParentId { get; set; }
            public string? OperationName { get; set; }
            public Activity? Activity { get; set; }
        }
    }
}
