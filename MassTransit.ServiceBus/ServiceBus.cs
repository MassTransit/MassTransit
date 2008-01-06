using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;
using MassTransit.ServiceBus.Util;

namespace MassTransit.ServiceBus
{
    public class ServiceBus :
        IServiceBus,
        IEnvelopeConsumer
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Dictionary<string, ServiceBusAsyncResult> _asyncResultDictionary =
            new Dictionary<string, ServiceBusAsyncResult>();

        private readonly Dictionary<Type, INotifyMessageConsumer> _consumers =
            new Dictionary<Type, INotifyMessageConsumer>();

        private readonly object _consumersLock = new object();

        private readonly ISubscriptionStorage _subscriptionStorage;
        private IReadWriteEndpoint _endpoint;

        public ServiceBus(IReadWriteEndpoint endpoint, ISubscriptionStorage subscriptionStorage)
        {
            Check.Parameter(endpoint).WithMessage("endpoint").IsNotNull();
            Check.Parameter(subscriptionStorage).WithMessage("subscriptionStorage").IsNotNull();

            _endpoint = endpoint;

            _subscriptionStorage = subscriptionStorage;
            
            _endpoint.Subscribe(this);

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Added event handler for envelope to {0}", _endpoint.Address);
        }

        public ISubscriptionStorage SubscriptionStorage
        {
            get { return _subscriptionStorage; }
        }

        #region IServiceBus Members

        public void Dispose()
        {
            _consumers.Clear();

            _endpoint.Dispose();

            _subscriptionStorage.Dispose();
        }

        public void Publish<T>(params T[] messages) where T : IMessage
        {
            IList<IEndpoint> subscribers = _subscriptionStorage.List<T>();
            if (subscribers.Count > 0)
            {
                foreach (IEndpoint endpoint in subscribers)
                {
                    IEnvelope envelope = new Envelope(Endpoint, messages as IMessage[]);

                    endpoint.Send(envelope);
                }
            }
        }

        public void Send<T>(IEndpoint endpoint, params T[] messages) where T : IMessage
        {
            IEnvelope envelope = new Envelope(Endpoint, messages as IMessage[]);

            endpoint.Send(envelope);
        }

        public IReadWriteEndpoint Endpoint
        {
            get { return _endpoint; }
        }


        public void Subscribe<T>(MessageReceivedCallback<T> callback) where T : IMessage
        {
            this.Consumer<T>().Subscribe(callback);
        }

        public void Subscribe<T>(MessageReceivedCallback<T> callback, Predicate<T> condition) where T : IMessage
        {
            Consumer<T>().Subscribe(callback, condition);
        }

        public IServiceBusAsyncResult Request<T>(IEndpoint endpoint, params T[] messages) where T : IMessage
        {
            IEnvelope envelope = new Envelope(Endpoint, messages as IMessage[]);

            lock (_asyncResultDictionary)
            {
                endpoint.Send(envelope);

                ServiceBusAsyncResult asyncResult = new ServiceBusAsyncResult();

                _log.DebugFormat("Recording request correlation ID {0}", envelope.Id);

                _asyncResultDictionary.Add(envelope.Id, asyncResult);

                return asyncResult;
            }
        }

        #endregion

        public IMessageConsumer<T> Consumer<T>() where T : IMessage
        {
            lock (_consumersLock)
            {
                if (!_consumers.ContainsKey(typeof(T)))
                {
                    _consumers[typeof(T)] = new MessageConsumer<T>();
                    _subscriptionStorage.Add(typeof(T), Endpoint);
                }

                return _consumers[typeof(T)] as IMessageConsumer<T>;
            }
        }

        private void MessageDoesntHaveCorrelationId(IEnvelope envelope)
        {
            if (envelope.Messages != null)
            {
                foreach (IMessage message in envelope.Messages)
                {
                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Message Received: {0}", message.GetType());

                    if (_consumers.ContainsKey(message.GetType()))
                    {
                        INotifyMessageConsumer receivingConsumer =
                            _consumers[message.GetType()];
                        if (receivingConsumer != null)
                        {
                            try
                            {
                                receivingConsumer.Deliver(this, envelope, message);
                            }
                            catch (Exception ex)
                            {
                                _log.Error("Exception from Deliver: ", ex);
                            }
                        }
                    }
                }
            }
        }

        private void MessageHasCorrelationId(IEnvelope envelope)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Correlation Id = {0}", envelope.CorrelationId);

            lock (_asyncResultDictionary)
            {
                if (_asyncResultDictionary.ContainsKey(envelope.CorrelationId))
                {
                    ServiceBusAsyncResult asyncResult = _asyncResultDictionary[envelope.CorrelationId];
                    _asyncResultDictionary.Remove(envelope.CorrelationId);

                        asyncResult.Complete(envelope.Messages);
                }
            }
        }

        public bool MeetsCriteria(IEnvelope envelope)
        {
            try
            {
                if (string.IsNullOrEmpty(envelope.CorrelationId))
                {
                    if (envelope.Messages != null)
                    {
                        foreach (IMessage message in envelope.Messages)
                        {
                            if (_consumers.ContainsKey(message.GetType()))
                            {
                                INotifyMessageConsumer receivingConsumer =
                                    _consumers[message.GetType()];

                                if ( receivingConsumer.MeetsCriteria(message) )
                                    return true;
                            }
                        }
                    }
                }
                else
                {
                    if (_asyncResultDictionary.ContainsKey(envelope.CorrelationId))
                        return true;
                }
            }
            catch (Exception ex)
            {
                _log.Error("Exception in Endpoint_EnvelopeReceived: ", ex);
            }

            return false;
        }

        public void Deliver(IEnvelope envelope)
        {
            try
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Envelope {0} Received By {1}", envelope.Id, GetHashCode());

                if (string.IsNullOrEmpty(envelope.CorrelationId))
                {
                    MessageDoesntHaveCorrelationId(envelope);
                }
                else
                {
                    MessageHasCorrelationId(envelope);
                }
            }
            catch (Exception ex)
            {
                _log.Error("Exception in Endpoint_EnvelopeReceived: ", ex);
            }
        }
    }
}