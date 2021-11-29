namespace MassTransit.Transports
{
    using System;


    public class ConsumeSendPipeAdapter<TMessage> :
        SendContextPipeAdapter<TMessage>
        where TMessage : class
    {
        readonly ConsumeContext _consumeContext;
        readonly Guid? _requestId;

        public ConsumeSendPipeAdapter(ConsumeContext consumeContext, IPipe<SendContext<TMessage>> pipe, Guid? requestId)
            : base(pipe)
        {
            _consumeContext = consumeContext;
            _requestId = requestId;
        }

        protected override void Send<T>(SendContext<T> context)
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
        }

        protected override void Send(SendContext<TMessage> context)
        {
        }
    }
}
