#nullable enable
namespace MassTransit.JobService
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using Contracts.JobService;


    public class FaultJobContext<TJob> :
        ConsumeContextProxy,
        ConsumeContext<TJob>
        where TJob : class
    {
        readonly ConsumeContext<FaultJob> _context;

        public FaultJobContext(ConsumeContext<FaultJob> context, TJob job)
            : base(context)
        {
            _context = context;

            Job = job;
        }

        public TJob Job { get; }

        public TJob Message => Job;

        public Task NotifyConsumed(TimeSpan duration, string consumerType)
        {
            return _context.NotifyConsumed(this, duration, consumerType);
        }

        public Task NotifyFaulted(TimeSpan duration, string consumerType, Exception exception)
        {
            return _context.NotifyFaulted(this, duration, consumerType, exception);
        }
    }
}
