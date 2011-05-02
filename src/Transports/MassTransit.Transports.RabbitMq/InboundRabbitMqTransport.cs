namespace MassTransit.Transports.RabbitMq
{
    using System;
    using log4net;
    using RabbitMQ.Client;
    using Util;

    public class InboundRabbitMqTransport : IInboundTransport
    {
        static readonly ILog _log = LogManager.GetLogger(typeof (InboundRabbitMqTransport));

        readonly RabbitMqAddress _address;
        readonly IConnection _connection;

        public InboundRabbitMqTransport(RabbitMqAddress address, IConnection connection)
        {
            _address = address;
            _connection = connection;
        }

        public IEndpointAddress Address { get; private set; }
        
        public void Receive(Func<IReceiveContext, Action<IReceiveContext>> callback, TimeSpan timeout)
        {
            //GuardAgainstDisposed();

            using (IModel channel = _connection.CreateModel())
            {
                channel.QueueDeclare(_address.Queue, true);
                BasicGetResult result = channel.BasicGet(_address.Queue, true);

                using (var context = new RabbitMqReceiveContext(result))
                {
                    Action<IReceiveContext> receive = callback(context);
                    if (receive == null)
                    {
                        if (_log.IsDebugEnabled)
                            _log.DebugFormat("SKIP:{0}:{1}", Address, result.BasicProperties.MessageId);

                        if (SpecialLoggers.Messages.IsInfoEnabled)
                            SpecialLoggers.Messages.InfoFormat("SKIP:{0}:{1}", Address, result.BasicProperties.MessageId);
                    }
                    else
                    {
                        receive(context);
                    }
                }

                channel.BasicAck(result.DeliveryTag, false);
                channel.Close(200, "ok");
            }
        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}