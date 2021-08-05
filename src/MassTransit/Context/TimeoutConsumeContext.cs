namespace MassTransit.Context
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public class TimeoutConsumeContext :
        ConsumeContextProxy
    {
        public TimeoutConsumeContext(ConsumeContext context, CancellationToken cancellationToken)
            : base(context)
        {
            CancellationToken = cancellationToken;
        }

        public override CancellationToken CancellationToken { get; }
    }


    public class TimeoutConsumeContext<T> :
        TimeoutConsumeContext,
        ConsumeContext<T>
        where T : class
    {
        readonly ConsumeContext<T> _context;

        public TimeoutConsumeContext(ConsumeContext<T> context, CancellationToken cancellationToken)
            : base(context, cancellationToken)
        {
            _context = context;
        }

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
