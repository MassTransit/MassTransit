namespace MassTransit.Middleware
{
    using System;
    using System.Threading.Tasks;


    public class DelaySendPipe<T> :
        IPipe<SendContext<T>>
        where T : class
    {
        readonly TimeSpan _delay;
        readonly IPipe<SendContext<T>> _pipe;

        public DelaySendPipe(IPipe<SendContext<T>> pipe, TimeSpan delay)
        {
            _pipe = pipe;
            _delay = delay;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            _pipe?.Probe(context);
        }

        public Task Send(SendContext<T> context)
        {
            if (_delay > TimeSpan.Zero)
                context.Delay = _delay;

            return _pipe.IsNotEmpty() ? _pipe.Send(context) : Task.CompletedTask;
        }
    }
}
