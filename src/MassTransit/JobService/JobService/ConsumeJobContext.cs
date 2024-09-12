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
        IDisposable
        where TJob : class
    {
        readonly ConsumeContext<StartJob> _context;
        readonly Uri _instanceAddress;
        readonly CancellationTokenSource _source;
        readonly Stopwatch _stopwatch;

        public ConsumeJobContext(ConsumeContext<StartJob> context, Uri instanceAddress, Guid jobId, Guid attemptId, int retryAttempt, TJob job,
            TimeSpan jobTimeout)
            : base(context)
        {
            _context = context;
            _instanceAddress = instanceAddress;

            JobId = jobId;
            Job = job;
            AttemptId = attemptId;
            RetryAttempt = retryAttempt;

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

        public void Dispose()
        {
            _source.Dispose();
        }

        public Guid JobId { get; }
        public Guid AttemptId { get; }
        public int RetryAttempt { get; }
        public TJob Job { get; }

        public TimeSpan ElapsedTime => _stopwatch.Elapsed;

        public Task NotifyCanceled(string? reason = null)
        {
            LogContext.Debug?.Log("Job Canceled: {JobId} {AttemptId} ({RetryAttempt})", JobId, AttemptId, RetryAttempt);

            return Notify<JobAttemptCanceled>(new JobAttemptCanceledEvent
            {
                JobId = JobId,
                AttemptId = AttemptId,
                RetryAttempt = RetryAttempt,
                Timestamp = DateTime.UtcNow,
            });
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

        public Task NotifyCompleted()
        {
            LogContext.Debug?.Log("Job Completed: {JobId} {AttemptId} ({RetryAttempt})", JobId, AttemptId, RetryAttempt);

            return Notify<JobAttemptCompleted>(new JobAttemptCompletedEvent
            {
                JobId = JobId,
                AttemptId = AttemptId,
                RetryAttempt = RetryAttempt,
                Timestamp = DateTime.UtcNow,
                Duration = ElapsedTime
            });
        }

        public Task NotifyFaulted(Exception exception, TimeSpan? delay)
        {
            LogContext.Debug?.Log(exception, "Job Faulted: {JobId} {AttemptId} ({RetryAttempt})", JobId, AttemptId, RetryAttempt);

            return Notify<JobAttemptFaulted>(new JobAttemptFaultedEvent
            {
                JobId = JobId,
                AttemptId = AttemptId,
                RetryAttempt = RetryAttempt,
                RetryDelay = delay,
                Timestamp = DateTime.UtcNow,
                Exceptions = new FaultExceptionInfo(exception)
            });
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
