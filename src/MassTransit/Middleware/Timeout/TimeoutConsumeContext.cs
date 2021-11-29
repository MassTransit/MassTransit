namespace MassTransit.Middleware.Timeout
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;


    public class TimeoutConsumeContext<T> :
        ConsumeContextProxy,
        ConsumeContext<T>
        where T : class
    {
        readonly ConsumeContext<T> _context;

        public TimeoutConsumeContext(ConsumeContext<T> context, CancellationToken cancellationToken)
            : base(context)
        {
            CancellationToken = cancellationToken;
            _context = context;
        }

        public override CancellationToken CancellationToken { get; }

        public T Message => _context.Message;

        public virtual Task NotifyConsumed(TimeSpan duration, string consumerType)
        {
            return NotifyConsumed(this, duration, consumerType);
        }

        public virtual Task NotifyFaulted(TimeSpan duration, string consumerType, Exception exception)
        {
            return NotifyFaulted(this, duration, consumerType, exception);
        }
    }
}
