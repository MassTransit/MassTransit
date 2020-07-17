namespace MassTransit.JobService.Components
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Courier;
    using MassTransit.Contracts.JobService;


    public class ConsumeJobContext<TJob> :
        ConsumeContextProxy,
        JobContext<TJob>,
        IDisposable
        where TJob : class
    {
        readonly ConsumeContext _context;
        readonly Uri _instanceAddress;
        readonly CancellationTokenSource _source;
        readonly Stopwatch _stopwatch;

        public ConsumeJobContext(ConsumeContext context, Uri instanceAddress, Guid jobId, Guid attemptId, int retryAttempt, TJob job, TimeSpan jobTimeout)
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

        public void Dispose()
        {
            _source.Dispose();
        }

        public override CancellationToken CancellationToken => _source.Token;

        public Guid JobId { get; }
        public Guid AttemptId { get; }
        public int RetryAttempt { get; }
        public TJob Job { get; }

        public TimeSpan ElapsedTime => _stopwatch.Elapsed;

        public Task NotifyCanceled(string reason = null)
        {
            LogContext.Debug?.Log("Job Canceled: {JobId} {AttemptId} ({RetryAttempt})", JobId, AttemptId, RetryAttempt);

            return Notify<JobAttemptCanceled>(new
            {
                JobId,
                AttemptId,
                RetryAttempt,
                InVar.Timestamp
            });
        }

        public Task NotifyStarted()
        {
            LogContext.Debug?.Log("Job Started: {JobId} {AttemptId} ({RetryAttempt})", JobId, AttemptId, RetryAttempt);

            return Notify<JobAttemptStarted>(new
            {
                JobId,
                AttemptId,
                RetryAttempt,
                InVar.Timestamp,
                InstanceAddress = _instanceAddress
            });
        }

        public Task NotifyCompleted()
        {
            LogContext.Debug?.Log("Job Completed: {JobId} {AttemptId} ({RetryAttempt})", JobId, AttemptId, RetryAttempt);

            return Notify<JobAttemptCompleted>(new
            {
                JobId,
                AttemptId,
                RetryAttempt,
                InVar.Timestamp,
                Duration = ElapsedTime
            });
        }

        public Task NotifyFaulted(Exception exception, TimeSpan? delay)
        {
            LogContext.Debug?.Log(exception, "Job Faulted: {JobId} {AttemptId} ({RetryAttempt})", JobId, AttemptId, RetryAttempt);

            return Notify<JobAttemptFaulted>(new
            {
                JobId,
                AttemptId,
                RetryAttempt,
                RetryDelay = delay,
                InVar.Timestamp,
                Exceptions = exception
            });
        }

        async Task Notify<T>(object values)
            where T : class
        {
            var endpoint = await _context.ReceiveContext.PublishEndpointProvider.GetPublishSendEndpoint<T>().ConfigureAwait(false);

            await endpoint.Send<T>(values).ConfigureAwait(false);
        }

        public void Cancel()
        {
            _source.Cancel();
        }
    }
}
