namespace MassTransit.ServiceBus.NMS
{
    using System;
    using Apache.NMS;
    using Apache.NMS.ActiveMQ;
    using Internal;

    public class NmsMessageSender :
        IMessageSender
    {
        private IConnectionFactory _factory;
        private readonly string _queueName;

        public NmsMessageSender(IEndpoint endpoint)
        {
            UriBuilder queueUri = new UriBuilder("tcp", endpoint.Uri.Host, endpoint.Uri.Port);

            _queueName = endpoint.Uri.AbsolutePath.Substring(1);

            _factory = new ConnectionFactory(queueUri.Uri);
        }

        public void Send(IEnvelope envelope)
        {
            using (IConnection connection = _factory.CreateConnection())
            {
                using (ISession session = connection.CreateSession())
                {
                    IDestination destination = session.GetQueue(_queueName);

                    IBytesMessage msg = NmsEnvelopeMapper.MapFrom(session, envelope);

                    using (IMessageProducer producer = session.CreateProducer(destination))
                    {
                        producer.Send(msg);
                        // TODO need to abstract IMessageId to support multiple platforms
                        // envelope.Id = msg.NMSMessageId;
                    }
                }

                //// lets consume a message
                //ActiveMQTextMessage message = (ActiveMQTextMessage)consumer.Receive();
                //if (message == null)
                //{
                //    Console.WriteLine("No message received!");
                //}
                //else
                //{
                //    Console.WriteLine("Received message with ID:   " + message.NMSMessageId);
                //    Console.WriteLine("Received message with text: " + message.Text);
                //}
            }
            //Message msg = MessageQueueEnvelopeMapper.MapFrom(envelope);

            //try
            //{
            //    if (_messageLog.IsInfoEnabled)
            //        _messageLog.InfoFormat("Message {0} Sent To {1}", envelope.Messages[0].GetType(), _endpoint.Uri);

            //    _queue.Send(msg);
            //}
            //catch (MessageQueueException ex)
            //{
            //    throw new EndpointException(_endpoint, "Problem with " + _endpoint.QueuePath, ex);
            //}

            //envelope.Id = msg.Id;

            //if (_log.IsDebugEnabled)
            //    _log.DebugFormat("Message Sent: Id = {0}, Message Type = {1}", msg.Id,
            //                     envelope.Messages != null ? envelope.Messages[0].GetType().ToString() : "");
        }

        public void Dispose()
        {
            if (_factory != null)
            {
                _factory = null;
            }
        }
    }
}