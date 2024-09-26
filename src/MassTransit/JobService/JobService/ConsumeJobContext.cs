#nullable enable
namespace MassTransit.JobService
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Contracts.JobService;
    using Events;
    using Messages;


    public class ConsumeJobContext<TJob> :
        ConsumeContextProxy,
        ConsumeContext<TJob>,
        JobContext<TJob>,
        INotifyJobContext,
        IAsyncDisposable
        where TJob : class
    {
        readonly ProgressBufferSettings _bufferSettings;
        readonly ConsumeContext<StartJob> _context;
        readonly Uri _instanceAddress;
        readonly CancellationTokenSource _source;
        readonly Stopwatch _stopwatch;
        JobProgressBuffer? _updateBuffer;

        public ConsumeJobContext(ConsumeContext<StartJob> context, Uri instanceAddress, TJob job, TimeSpan jobTimeout, ProgressBufferSettings bufferSettings)
            : base(context)
        {
            _context = context;
            _instanceAddress = instanceAddress;
            _bufferSettings = bufferSettings;

            JobId = context.Message.JobId;
            AttemptId = context.Message.AttemptId;
            RetryAttempt = context.Message.RetryAttempt;

            Job = job;

            LastProgressValue = context.Message.LastProgressValue;
            LastProgressLimit = context.Message.LastProgressLimit;

            _source = new CancellationTokenSource(jobTimeout);
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

        public async Task NotifyCanceled(string? reason = null)
        {
            LogContext.Debug?.Log("Job Canceled: {JobId} {AttemptId} ({RetryAttempt})", JobId, AttemptId, RetryAttempt);

            if (_updateBuffer != null)
                await _updateBuffer.Flush().ConfigureAwait(false);

            await Notify<JobAttemptCanceled>(new JobAttemptCanceledEvent
            {
                JobId = JobId,
                AttemptId = AttemptId,
                RetryAttempt = RetryAttempt,
                Timestamp = DateTime.UtcNow,
            }).ConfigureAwait(false);
        }

        public Task NotifyStarted()
        {
            LogContext.Debug?.Log("Job Started: {JobId} {AttemptId} ({RetryAttempt})", JobId, AttemptId, RetryAttempt);

            return Notify<JobAttemptStarted>(new JobAttemptStartedEvent
            {
                JobId = JobId,
                AttemptId = AttemptId,
                RetryAttempt = RetryAttempt,
                Timestamp = DateTime.UtcNow,
                InstanceAddress = _instanceAddress
            });
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
            _updateBuffer ??= new JobProgressBuffer(this, _bufferSettings);

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

        public bool TryGetJobState<T>(out T? jobState)
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

        async Task Notify<T>(T message)
            where T : class
        {
            var endpoint = await _context.ReceiveContext.PublishEndpointProvider.GetPublishSendEndpoint<T>().ConfigureAwait(false);

            await endpoint.Send(message, CancellationToken.None).ConfigureAwait(false);
        }

        public void Cancel()
        {
            _source.Cancel();
        }
    }
}
