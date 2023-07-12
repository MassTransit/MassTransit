namespace MassTransit.Middleware.Timeout
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;


    public class TimeoutConsumeContext<TMessage> :
        ConsumeContextProxy,
        ConsumeContext<TMessage>
        where TMessage : class
    {
        readonly ConsumeContext<TMessage> _context;

        public TimeoutConsumeContext(ConsumeContext<TMessage> context, CancellationToken cancellationToken)
            : base(context)
        {
            CancellationToken = cancellationToken;
            _context = context;
        }

        public override CancellationToken CancellationToken { get; }

        public TMessage Message => _context.Message;

        public override async Task NotifyFaulted<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
        {
            switch (exception)
            {
                case OperationCanceledException canceledException when canceledException.CancellationToken == _context.CancellationToken:
                    break;

                default:
                    if (!_context.CancellationToken.IsCancellationRequested)
                        await GenerateFault(_context, exception).ConfigureAwait(false);
                    break;
            }

            await ReceiveContext.NotifyFaulted(context, duration, consumerType, exception).ConfigureAwait(false);
        }

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
