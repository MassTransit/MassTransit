namespace MassTransit.Scheduling
{
    using System;
    using Transports;


    /// <summary>
    /// For transport-based schedulers, used to invoke the <see cref="SendContext{T}" /> pipe and
    /// manage the ScheduledMessageId, as well as set the transport delay property
    /// </summary>
    /// <typeparam name="TMessage">The message type</typeparam>
    public class ScheduleSendPipe<TMessage> :
        SendContextPipeAdapter<TMessage>
        where TMessage : class
    {
        readonly DateTime _scheduledTime;
        SendContext _context;

        Guid? _scheduledMessageId;

        public ScheduleSendPipe(IPipe<SendContext<TMessage>> pipe, DateTime scheduledTime)
            : base(pipe)
        {
            _scheduledTime = scheduledTime;
        }

        public Guid? ScheduledMessageId
        {
            get => _context?.ScheduledMessageId ?? _scheduledMessageId;
            set => _scheduledMessageId = value;
        }

        public Guid? MessageId => _context?.MessageId;

        protected override void Send(SendContext<TMessage> context)
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
        }

        protected override void Send<T>(SendContext<T> context)
        {
        }
    }
}
