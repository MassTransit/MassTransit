namespace MassTransit.Context
{
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Util;
    using Pipeline;


    public struct ConsumeSendContextPipe<TMessage> :
        IPipe<SendContext<TMessage>>,
        ISendContextPipe
        where TMessage : class
    {
        readonly ConsumeContext _consumeContext;
        readonly IPipe<SendContext<TMessage>> _pipe;

        public ConsumeSendContextPipe(ConsumeContext consumeContext)
        {
            _consumeContext = consumeContext;

            _pipe = default;
        }

        public ConsumeSendContextPipe(ConsumeContext consumeContext, IPipe<SendContext<TMessage>> pipe)
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

        public Task Send(SendContext<TMessage> context)
        {
            return _pipe?.Send(context) ?? TaskUtil.Completed;
        }

        public Task Send<T>(SendContext<T> context)
            where T : class
        {
            if (_consumeContext != null)
                context.TransferConsumeContextHeaders(_consumeContext);

            return TaskUtil.Completed;
        }
    }
}
