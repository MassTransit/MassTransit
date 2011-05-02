namespace MassTransit.Transports.RabbitMq
{
    using System;
    using RabbitMQ.Client;
    using Util;

    public class OutboundRabbitMqTransport : IOutboundTransport
    {
        readonly RabbitMqAddress _address;
        readonly IConnection _connection;

        public OutboundRabbitMqTransport(RabbitMqAddress address, IConnection connection)
        {
            _address = address;
            _connection = connection;
        }

        public IEndpointAddress Address
        {
            get { return _address; }
        }

        public void Send(Action<ISendContext> callback)
        {
            //GuardAgainstDisposed();

            using (IModel channel = _connection.CreateModel())
            {
                channel.ExchangeDeclare(_address.Queue, "fanout", true);

                using (var context = new RabbitMqSendContext(channel))
                {
                    callback(context);

                    channel.BasicPublish(_address.Queue, "msg", context.Properties, context.GetBytes());

                    if (SpecialLoggers.Messages.IsInfoEnabled)
                        SpecialLoggers.Messages.InfoFormat("SEND:{0}:{1}", Address, context.Properties.MessageId);
                }

                channel.Close(200, "ok");
            }
        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}