namespace MassTransit.RabbitMqTransport
{
    using Transports;


    public class ReplyToSendEndpoint :
        SendEndpointProxy
    {
        readonly string _queueName;

        public ReplyToSendEndpoint(ISendEndpoint endpoint, string queueName)
            : base(endpoint)
        {
            _queueName = queueName;
        }

        protected override IPipe<SendContext<T>> GetPipeProxy<T>(IPipe<SendContext<T>> pipe = default)
        {
            return new ReplyToPipe<T>(_queueName, pipe);
        }


        class ReplyToPipe<TMessage> :
            SendContextPipeAdapter<TMessage>
            where TMessage : class
        {
            readonly string _queueName;

            public ReplyToPipe(string queueName, IPipe<SendContext<TMessage>> pipe)
                : base(pipe)
            {
                _queueName = queueName;
            }

            protected override void Send(SendContext<TMessage> context)
            {
                context.SetRoutingKey(_queueName);
            }

            protected override void Send<T>(SendContext<T> context)
            {
            }
        }
    }
}
