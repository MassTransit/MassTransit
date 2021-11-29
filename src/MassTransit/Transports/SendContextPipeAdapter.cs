namespace MassTransit.Transports
{
    using System.Threading.Tasks;


    public abstract class SendContextPipeAdapter<TMessage> :
        IPipe<SendContext<TMessage>>,
        ISendPipe
        where TMessage : class
    {
        readonly IPipe<SendContext<TMessage>> _pipe;

        protected SendContextPipeAdapter(IPipe<SendContext<TMessage>> pipe)
        {
            _pipe = pipe;
        }

        public void Probe(ProbeContext context)
        {
            _pipe?.Probe(context);
        }

        Task IPipe<SendContext<TMessage>>.Send(SendContext<TMessage> context)
        {
            Send(context);

            return _pipe.IsNotEmpty() ? _pipe.Send(context) : Task.CompletedTask;
        }

        Task ISendContextPipe.Send<T>(SendContext<T> context)
            where T : class
        {
            Send(context);

            return _pipe is ISendContextPipe sendContextPipe
                ? sendContextPipe.Send(context)
                : Task.CompletedTask;
        }

        protected abstract void Send(SendContext<TMessage> context);

        protected abstract void Send<T>(SendContext<T> context)
            where T : class;
    }
}
