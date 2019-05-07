namespace MassTransit.Context
{
    using System.Threading.Tasks;
    using GreenPipes;


    public struct ConsumeSendContextPipe<T> :
        IPipe<SendContext<T>>
        where T : class
    {
        readonly ConsumeContext _consumeContext;
        readonly IPipe<SendContext<T>> _pipe;

        public ConsumeSendContextPipe(ConsumeContext consumeContext)
        {
            _consumeContext = consumeContext;

            _pipe = default;
        }

        public ConsumeSendContextPipe(ConsumeContext consumeContext, IPipe<SendContext<T>> pipe)
        {
            _consumeContext = consumeContext;
            _pipe = pipe;
        }

        public ConsumeSendContextPipe(ConsumeContext consumeContext, IPipe<SendContext> pipe)
        {
            _consumeContext = consumeContext;
            _pipe = pipe;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            _pipe?.Probe(context);
        }

        public async Task Send(SendContext<T> context)
        {
            if (_consumeContext != null)
                context.TransferConsumeContextHeaders(_consumeContext);

            if (_pipe.IsNotEmpty())
                await _pipe.Send(context).ConfigureAwait(false);
        }
    }
}