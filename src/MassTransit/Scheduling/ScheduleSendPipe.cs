namespace MassTransit.Scheduling
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;


    public class ScheduleSendPipe<T> :
        IPipe<SendContext<T>>
        where T : class
    {
        readonly IPipe<SendContext<T>> _pipe;
        SendContext _context;

        Guid? _scheduledMessageId;

        protected ScheduleSendPipe(IPipe<SendContext<T>> pipe)
        {
            _pipe = pipe;
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

            if (_pipe.IsNotEmpty())
                await _pipe.Send(context).ConfigureAwait(false);
        }
    }
}
