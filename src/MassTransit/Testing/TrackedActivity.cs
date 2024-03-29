#nullable enable
namespace MassTransit.Testing;

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Util;


class TrackedActivity :
    IAsyncDisposable
{
    static readonly ActivitySource _source = new ActivitySource("MassTransit.Testing.Monitor");

    readonly TaskCompletionSource<bool> _completed;
    readonly TimeSpan _idleTimeout;
    readonly ActivityListener _listener;
    readonly Activity? _testActivity;
    readonly TimeSpan _timeout;
    readonly RollingTimer _timer;
    readonly TraceInfo _traceInfo;

    public TrackedActivity(string? methodName, TimeSpan? timeout, TimeSpan? idleTimeout)
    {
        _idleTimeout = idleTimeout ?? TimeSpan.FromSeconds(0.05);
        _timeout = timeout ?? TimeSpan.FromSeconds(30);

        Activity.DefaultIdFormat = ActivityIdFormat.W3C;
        Activity.ForceDefaultIdFormat = true;

        _completed = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        _listener = new ActivityListener
        {
            ShouldListenTo = _ => true,
            Sample = Sample,
            ActivityStarted = ActivityStarted,
            ActivityStopped = ActivityStopped
        };

        ActivitySource.AddActivityListener(_listener);

        _testActivity = _source.StartActivity($"{methodName ?? "test"} process");
        _traceInfo = new TraceInfo { StartTime = _testActivity?.StartTimeUtc ?? DateTimeOffset.UtcNow };
        _timer = new RollingTimer(OnTimeout, _timeout, this);
        _timer.Start();
    }

    public async ValueTask DisposeAsync()
    {
        await _completed.Task.ConfigureAwait(false);

        _testActivity?.Stop();

        _testActivity?.Dispose();

        _listener.Dispose();

        _timer.Dispose();
    }

    void OnTimeout(object? state)
    {
        _completed.TrySetResult(true);
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
        if (span != null)
            span.Completed = true;

        if (_traceInfo.Spans.All(x => x.Value.Completed))
            _timer.Restart(_idleTimeout);
    }

    SpanInfo? GetSpan(Activity activity)
    {
        var traceId = activity.RootId ?? "";
        if (traceId != _testActivity?.RootId)
            return null;

        var span = _traceInfo.Spans.GetOrAdd(activity.Id ?? "", id => new SpanInfo
        {
            SpanId = id,
            ParentId = activity.ParentId,
            StartTime = activity.StartTimeUtc,
            OperationName = activity.OperationName,
            Activity = activity,
        });

        if (activity.Duration > TimeSpan.Zero)
        {
            span.Duration = activity.Duration;

            var traceDuration = activity.StartTimeUtc - _traceInfo.StartTime + activity.Duration;

            if (traceDuration > _traceInfo.Duration)
                _traceInfo.Duration = traceDuration;
        }

        return span;
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
        public bool Completed { get; set; }
    }
}
