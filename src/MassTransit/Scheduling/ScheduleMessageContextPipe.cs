namespace MassTransit.Scheduling
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;


    public class ScheduleMessageContextPipe<T> :
        IPipe<SendContext<T>>,
        IPipe<SendContext<ScheduleMessage<T>>>
        where T : class
    {
        readonly IPipe<SendContext<T>> _pipe;
        SendContext _context;

        Guid? _scheduledMessageId;

        protected ScheduleMessageContextPipe()
        {
        }

        public ScheduleMessageContextPipe(IPipe<SendContext<T>> pipe)
        {
            _pipe = pipe;
        }

        public Guid? ScheduledMessageId
        {
            get => _context?.ScheduledMessageId ?? _scheduledMessageId;
            set => _scheduledMessageId = value;
        }

        public virtual async Task Send(SendContext<ScheduleMessage<T>> context)
        {
            _context = context;

            _context.ScheduledMessageId = _scheduledMessageId;

            if (_pipe.IsNotEmpty())
            {
                SendContext<T> proxy = context.CreateProxy(context.Message.Payload);

                await _pipe.Send(proxy).ConfigureAwait(false);
            }
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            _pipe?.Probe(context);
        }

        public virtual async Task Send(SendContext<T> context)
        {
            _context = context;

            _context.ScheduledMessageId = _scheduledMessageId;

            if (_pipe.IsNotEmpty())
                await _pipe.Send(context).ConfigureAwait(false);
        }
    }


    public class ScheduleMessageContextPipe :
        IPipe<SendContext>
    {
        readonly IPipe<SendContext> _pipe;
        SendContext _context;

        Guid? _scheduledMessageId;

        protected ScheduleMessageContextPipe()
        {
        }

        public ScheduleMessageContextPipe(IPipe<SendContext> pipe)
        {
            _pipe = pipe;
        }

        public Guid? ScheduledMessageId
        {
            get => _context?.ScheduledMessageId ?? _scheduledMessageId;
            set => _scheduledMessageId = value;
        }

        public virtual async Task Send(SendContext context)
        {
            _context = context;

            _context.ScheduledMessageId = _scheduledMessageId;

            if (_pipe.IsNotEmpty())
                await _pipe.Send(context).ConfigureAwait(false);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            _pipe?.Probe(context);
        }
    }
}
