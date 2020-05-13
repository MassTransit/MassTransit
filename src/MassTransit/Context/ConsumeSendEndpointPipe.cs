namespace MassTransit.Context
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Util;
    using Pipeline;


    public class ConsumeSendEndpointPipe<TMessage> :
        IPipe<SendContext<TMessage>>,
        ISendContextPipe
        where TMessage : class
    {
        readonly ConsumeContext _consumeContext;
        readonly IPipe<SendContext<TMessage>> _pipe;
        readonly Guid? _requestId;

        public ConsumeSendEndpointPipe(ConsumeContext consumeContext, Guid? requestId)
        {
            _consumeContext = consumeContext;
            _requestId = requestId;

            _pipe = default;
        }

        public ConsumeSendEndpointPipe(ConsumeContext consumeContext, IPipe<SendContext<TMessage>> pipe, Guid? requestId)
        {
            _consumeContext = consumeContext;
            _pipe = pipe;
            _requestId = requestId;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            _pipe?.Probe(context);
        }

        public Task Send(SendContext<TMessage> context)
        {
            return _pipe.IsNotEmpty() ? _pipe.Send(context) : TaskUtil.Completed;
        }

        public Task Send<T>(SendContext<T> context)
            where T : class
        {
            if (_requestId.HasValue)
                context.RequestId = _requestId;

            if (_consumeContext != null)
            {
                context.TransferConsumeContextHeaders(_consumeContext);

                if (_requestId.HasValue && _consumeContext.ExpirationTime.HasValue
                    && _consumeContext.ResponseAddress != null && _consumeContext.ResponseAddress == context.DestinationAddress)
                {
                    context.TimeToLive = _consumeContext.ExpirationTime.Value - DateTime.UtcNow;
                    if (context.TimeToLive.Value <= TimeSpan.Zero)
                        context.TimeToLive = TimeSpan.FromSeconds(1);
                }
            }

            return TaskUtil.Completed;
        }
    }
}
