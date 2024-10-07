#nullable enable
namespace MassTransit.JobService;

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Context;
using Contracts.JobService;
using Events;
using Messages;
using Serialization;


public class ConsumeJobContext<TJob> :
    ConsumeContextProxy,
    ConsumeContext<TJob>,
    JobContext<TJob>,
    INotifyJobContext,
    IAsyncDisposable
    where TJob : class
{
    readonly ConsumeContext<StartJob> _context;
    readonly Uri _instanceAddress;
    readonly JobOptions<TJob> _jobOptions;
    readonly CancellationTokenSource _source;
    readonly Stopwatch _stopwatch;
    string? _cancellationReason;
    JobProgressBuffer? _updateBuffer;

    public ConsumeJobContext(ConsumeContext<StartJob> context, Uri instanceAddress, TJob job, JobOptions<TJob> jobOptions)
        : base(context)
    {
        _context = context;
        _instanceAddress = instanceAddress;
        _jobOptions = jobOptions;

        JobId = context.Message.JobId;
        AttemptId = context.Message.AttemptId;
        RetryAttempt = context.Message.RetryAttempt;

        Job = job;

        LastProgressValue = context.Message.LastProgressValue;
        LastProgressLimit = context.Message.LastProgressLimit;

        var jobProperties = new JobPropertyCollection();
        jobProperties.SetMany(context.Message.JobProperties!);

        JobProperties = jobProperties;

        _source = new CancellationTokenSource(jobOptions.JobTimeout);
        _stopwatch = Stopwatch.StartNew();
    }

    public override CancellationToken CancellationToken => _source.Token;

    public TJob Message => Job;

    public Task NotifyConsumed(TimeSpan duration, string consumerType)
    {
        return _context.NotifyConsumed(_context, duration, consumerType);
    }

    public Task NotifyFaulted(TimeSpan duration, string consumerType, Exception exception)
    {
        return _context.NotifyFaulted(_context, duration, consumerType, exception);
    }

    public async ValueTask DisposeAsync()
    {
        if (_updateBuffer != null)
            await _updateBuffer.Flush().ConfigureAwait(false);

        _source.Dispose();
    }

    public async Task NotifyCanceled()
    {
        LogContext.Debug?.Log("Job Canceled: {JobId} {AttemptId} ({RetryAttempt}) {Reason}", JobId, AttemptId, RetryAttempt, _cancellationReason);

        if (_updateBuffer != null)
            await _updateBuffer.Flush().ConfigureAwait(false);

        await Notify<JobAttemptCanceled>(new JobAttemptCanceledEvent
        {
            JobId = JobId,
            AttemptId = AttemptId,
            Timestamp = DateTime.UtcNow,
            Reason = _cancellationReason ?? JobCancellationReasons.ConsumerInitiated
        }).ConfigureAwait(false);
    }

    public async Task NotifyStarted()
    {
        LogContext.Debug?.Log("Job Started: {JobId} {AttemptId} ({RetryAttempt})", JobId, AttemptId, RetryAttempt);

        var timestamp = DateTime.UtcNow;

        await Notify<JobAttemptStarted>(new JobAttemptStartedEvent
        {
            JobId = JobId,
            AttemptId = AttemptId,
            RetryAttempt = RetryAttempt,
            Timestamp = timestamp,
            InstanceAddress = _instanceAddress
        }).ConfigureAwait(false);

        var endpoint = await _context.ReceiveContext.PublishEndpointProvider.GetPublishSendEndpoint<JobStarted<TJob>>().ConfigureAwait(false);

        await endpoint.Send<JobStarted<TJob>>(new JobStartedEvent<TJob>
        {
            JobId = JobId,
            AttemptId = AttemptId,
            RetryAttempt = RetryAttempt,
            Timestamp = timestamp
        }, CancellationToken.None).ConfigureAwait(false);
    }

    public async Task NotifyCompleted()
    {
        LogContext.Debug?.Log("Job Completed: {JobId} {AttemptId} ({RetryAttempt})", JobId, AttemptId, RetryAttempt);

        if (_updateBuffer != null)
            await _updateBuffer.Flush().ConfigureAwait(false);

        await Notify<JobAttemptCompleted>(new JobAttemptCompletedEvent
        {
            JobId = JobId,
            AttemptId = AttemptId,
            RetryAttempt = RetryAttempt,
            Timestamp = DateTime.UtcNow,
            Duration = ElapsedTime
        }).ConfigureAwait(false);
    }

    public Task NotifyJobProgress(SetJobProgress progress)
    {
        return Notify(progress);
    }

    public async Task NotifyFaulted(Exception exception, TimeSpan? delay)
    {
        LogContext.Debug?.Log(exception, "Job Faulted: {JobId} {AttemptId} ({RetryAttempt})", JobId, AttemptId, RetryAttempt);

        if (_updateBuffer != null)
            await _updateBuffer.Flush().ConfigureAwait(false);

        await Notify<JobAttemptFaulted>(new JobAttemptFaultedEvent
        {
            JobId = JobId,
            AttemptId = AttemptId,
            RetryAttempt = RetryAttempt,
            RetryDelay = delay,
            Timestamp = DateTime.UtcNow,
            Exceptions = new FaultExceptionInfo(exception)
        }).ConfigureAwait(false);
    }

    public Guid JobId { get; }
    public Guid AttemptId { get; }
    public int RetryAttempt { get; }
    public long? LastProgressValue { get; }
    public long? LastProgressLimit { get; }
    public TJob Job { get; }

    public TimeSpan ElapsedTime => _stopwatch.Elapsed;

    public Task SetJobProgress(long value, long? limit)
    {
        _updateBuffer ??= new JobProgressBuffer(this, _jobOptions.ProgressBuffer);

        return _updateBuffer.Update(new JobProgressBuffer.ProgressUpdate(JobId, AttemptId, value, limit), CancellationToken.None);
    }

    public Task SaveJobState<T>(T? jobState)
        where T : class
    {
        return Notify<SaveJobState>(new SaveJobStateCommand
        {
            JobId = JobId,
            AttemptId = AttemptId,
            JobState = jobState != null ? _context.ToDictionary(jobState) : null
        });
    }

    public bool TryGetJobState<T>([NotNullWhen(true)] out T? jobState)
        where T : class
    {
        if (_context.Message.JobState != null)
        {
            jobState = _context.SerializerContext.DeserializeObject<T>(_context.Message.JobState);
            return jobState != null;
        }

        jobState = null;
        return false;
    }

    public IPropertyCollection JobProperties { get; set; }
    public IPropertyCollection JobTypeProperties => _jobOptions.JobTypeProperties;
    public IPropertyCollection InstanceProperties => _jobOptions.InstanceProperties;

    async Task Notify<T>(T message)
        where T : class
    {
        var endpoint = await _context.ReceiveContext.PublishEndpointProvider.GetPublishSendEndpoint<T>().ConfigureAwait(false);

        await endpoint.Send(message, CancellationToken.None).ConfigureAwait(false);
    }

    public void Cancel(string? reason)
    {
        _cancellationReason = reason;
        _source.Cancel();
    }
}
