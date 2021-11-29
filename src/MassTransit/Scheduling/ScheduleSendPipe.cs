namespace MassTransit.Scheduling
{
    using System;
    using System.Threading.Tasks;


    /// <summary>
    /// For transport-based schedulers, used to invoke the <see cref="SendContext{T}" /> pipe and
    /// manage the ScheduledMessageId, as well as set the transport delay property
    /// </summary>
    /// <typeparam name="T">The message type</typeparam>
    public class ScheduleSendPipe<T> :
        IPipe<SendContext<T>>
        where T : class
    {
        readonly IPipe<SendContext<T>> _pipe;
        readonly DateTime _scheduledTime;
        SendContext _context;

        Guid? _scheduledMessageId;

        public ScheduleSendPipe(IPipe<SendContext<T>> pipe, DateTime scheduledTime)
        {
            _pipe = pipe;
            _scheduledTime = scheduledTime;
        }

        public Guid? ScheduledMessageId
        {
            get => _context?.ScheduledMessageId ?? _scheduledMessageId;
            set => _scheduledMessageId = value;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            _pipe?.Probe(context);
        }

        public virtual async Task Send(SendContext<T> context)
        {
            _context = context;
            _context.ScheduledMessageId = _scheduledMessageId;

            var delay = _scheduledTime.Kind == DateTimeKind.Local
                ? _scheduledTime - DateTime.Now
                : _scheduledTime - DateTime.UtcNow;

            if (delay > TimeSpan.Zero)
                context.Delay = delay;

            if (ScheduledMessageId.HasValue)
                context.Headers.Set(MessageHeaders.SchedulingTokenId, ScheduledMessageId.Value.ToString("D"));

            if (_pipe.IsNotEmpty())
                await _pipe.Send(context).ConfigureAwait(false);
        }
    }
}
