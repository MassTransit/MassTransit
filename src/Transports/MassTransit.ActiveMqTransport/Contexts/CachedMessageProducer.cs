namespace MassTransit.ActiveMqTransport.Contexts
{
    using System;
    using Apache.NMS;
    using GreenPipes.Caching;


    public class CachedMessageProducer :
        IMessageProducer,
        INotifyValueUsed
    {
        readonly IMessageProducer _producer;

        public CachedMessageProducer(IDestination destination, IMessageProducer producer)
        {
            Destination = destination;
            _producer = producer;
        }

        public IDestination Destination { get; }

        public void Dispose()
        {
            _producer.Dispose();
        }

        public void Send(IMessage message)
        {
            Used?.Invoke();
            _producer.Send(message);
        }

        public void Send(IMessage message, MsgDeliveryMode deliveryMode, MsgPriority priority, TimeSpan timeToLive)
        {
            Used?.Invoke();
            _producer.Send(message, deliveryMode, priority, timeToLive);
        }

        public void Send(IDestination destination, IMessage message)
        {
            Used?.Invoke();
            _producer.Send(destination, message);
        }

        public void Send(IDestination destination, IMessage message, MsgDeliveryMode deliveryMode, MsgPriority priority, TimeSpan timeToLive)
        {
            Used?.Invoke();
            _producer.Send(destination, message, deliveryMode, priority, timeToLive);
        }

        public void Close()
        {
            _producer.Close();
        }

        public IMessage CreateMessage()
        {
            Used?.Invoke();
            return _producer.CreateMessage();
        }

        public ITextMessage CreateTextMessage()
        {
            Used?.Invoke();
            return _producer.CreateTextMessage();
        }

        public ITextMessage CreateTextMessage(string text)
        {
            Used?.Invoke();
            return _producer.CreateTextMessage(text);
        }

        public IMapMessage CreateMapMessage()
        {
            Used?.Invoke();
            return _producer.CreateMapMessage();
        }

        public IObjectMessage CreateObjectMessage(object body)
        {
            Used?.Invoke();
            return _producer.CreateObjectMessage(body);
        }

        public IBytesMessage CreateBytesMessage()
        {
            Used?.Invoke();
            return _producer.CreateBytesMessage();
        }

        public IBytesMessage CreateBytesMessage(byte[] body)
        {
            Used?.Invoke();
            return _producer.CreateBytesMessage(body);
        }

        public IStreamMessage CreateStreamMessage()
        {
            Used?.Invoke();
            return _producer.CreateStreamMessage();
        }

        public ProducerTransformerDelegate ProducerTransformer
        {
            get => _producer.ProducerTransformer;
            set => _producer.ProducerTransformer = value;
        }

        public MsgDeliveryMode DeliveryMode
        {
            get => _producer.DeliveryMode;
            set => _producer.DeliveryMode = value;
        }

        public TimeSpan TimeToLive
        {
            get => _producer.TimeToLive;
            set => _producer.TimeToLive = value;
        }

        public TimeSpan RequestTimeout
        {
            get => _producer.RequestTimeout;
            set => _producer.RequestTimeout = value;
        }

        public MsgPriority Priority
        {
            get => _producer.Priority;
            set => _producer.Priority = value;
        }

        public bool DisableMessageID
        {
            get => _producer.DisableMessageID;
            set => _producer.DisableMessageID = value;
        }

        public bool DisableMessageTimestamp
        {
            get => _producer.DisableMessageTimestamp;
            set => _producer.DisableMessageTimestamp = value;
        }

        public event Action Used;
    }
}
