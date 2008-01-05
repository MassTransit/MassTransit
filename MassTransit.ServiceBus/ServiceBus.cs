using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;
using MassTransit.ServiceBus.Util;

namespace MassTransit.ServiceBus
{
    public class ServiceBus :
        IServiceBus
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly object _messageEndpointLock = new object();
        private readonly Dictionary<Type, IEndpoint> _messageEndpoints = new Dictionary<Type, IEndpoint>();

        private IEndpoint _endpoint;

        private readonly ISubscriptionStorage _subscriptionStorage;
        private readonly Dictionary<string, ServiceBusAsyncResult> _asyncResultDictionary = new Dictionary<string, ServiceBusAsyncResult>();

        public ServiceBus(IEndpoint endpoint, ISubscriptionStorage subscriptionStorage)
        {
            Check.Parameter(endpoint).WithMessage("endpoint").IsNotNull();
            Check.Parameter(subscriptionStorage).WithMessage("subscriptionStorage").IsNotNull();

            _endpoint = endpoint;
            _endpoint.EnvelopeReceived += Transport_EnvelopeReceived;

            _subscriptionStorage = subscriptionStorage;

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
            _messageEndpoints.Clear();

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
                    IEnvelopeFactory envelopeFactory = endpoint as IEnvelopeFactory;
                    if (envelopeFactory != null)
                    {
                        IEnvelope envelope = envelopeFactory.NewEnvelope(Endpoint, messages as IMessage[]);

                        endpoint.Send(envelope);
                    }
                }
            }
        }

        public void Send<T>(IEndpoint endpoint, params T[] messages) where T : IMessage
        {
            IEnvelopeFactory envelopeFactory = endpoint as IEnvelopeFactory;
            if (envelopeFactory == null)
                throw new ArgumentNullException("endpoint", "Envelope Factory Not Supported");

            IEnvelope envelope = envelopeFactory.NewEnvelope(Endpoint, messages as IMessage[]);

            endpoint.Send(envelope);
        }

        public IEndpoint Endpoint
        {
            get { return _endpoint; }
            set { _endpoint = value; }
        }


        public void Subscribe<T>(MessageReceivedCallback<T> callback) where T : IMessage
        {
            this.MessageEndpoint<T>().Subscribe(callback);
        }

        public void Subscribe<T>(MessageReceivedCallback<T> callback, Predicate<T> condition) where T : IMessage
        {
            this.MessageEndpoint<T>().Subscribe(callback, condition);
        }

        public IMessageEndpoint<T> MessageEndpoint<T>() where T : IMessage
        {
            lock (_messageEndpointLock)
            {
                IEndpoint result;
                _messageEndpoints.TryGetValue(typeof (T), out result);

                if (result == null)
                {
                    _messageEndpoints[typeof (T)] = new MessageEndpoint<T>(Endpoint);

                    result = _messageEndpoints[typeof (T)];

                    _subscriptionStorage.Add(typeof (T), Endpoint);
                }

                return result as IMessageEndpoint<T>;
            }
        }

        public IServiceBusAsyncResult Request<T>(IEndpoint endpoint, params T[] messages) where T : IMessage
        {
            IEnvelopeFactory envelopeFactory = endpoint as IEnvelopeFactory;
            if (envelopeFactory == null)
                throw new ArgumentException("Endpoint does not support IEnvelopeFactory");

            IEnvelope envelope = envelopeFactory.NewEnvelope(Endpoint, messages as IMessage[]);

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

        private void Transport_EnvelopeReceived(object sender, EnvelopeReceivedEventArgs e)
        {
            try
            {
                if(_log.IsDebugEnabled)
                    _log.DebugFormat("Envelope {0} Received By {1}", e.Envelope.Id, GetHashCode());

                if(string.IsNullOrEmpty(e.Envelope.CorrelationId ))
                {
                    MessageDoesntHaveCorrelationId(e);
                }
                else
                {
                    MessageHasCorrelationId(e, sender);    
                }
            }
            catch (Exception ex)
            {
                _log.Error("Exception in Transport_EnvelopeReceived: ", ex);
            }
        }

        private void MessageDoesntHaveCorrelationId(EnvelopeReceivedEventArgs e)
        {
            if (e.Envelope.Messages != null)
            {
                foreach (IMessage message in e.Envelope.Messages)
                {
                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Message Received: {0}", message.GetType());

                    if (_messageEndpoints.ContainsKey(message.GetType()))
                    {
                        IMessageEndpointReceive receivingEndpoint =
                            _messageEndpoints[message.GetType()] as IMessageEndpointReceive;
                        if (receivingEndpoint != null)
                        {
                            try
                            {
                                receivingEndpoint.OnMessageReceived(this, e.Envelope, message);
                            }
                            catch (Exception ex)
                            {
                                _log.Error("Exception from OnMessageReceived: ", ex);
                            }
                        }
                    }
                }
            }
        }

        private void MessageHasCorrelationId(EnvelopeReceivedEventArgs e, object sender)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Correlation Id = {0}", e.Envelope.CorrelationId);

            lock (_asyncResultDictionary)
            {
                if (_asyncResultDictionary.ContainsKey(e.Envelope.CorrelationId))
                {
                    ServiceBusAsyncResult asyncResult = _asyncResultDictionary[e.Envelope.CorrelationId];
                    _asyncResultDictionary.Remove(e.Envelope.CorrelationId);

                    IEndpoint sourceEndpoint = sender as IEndpoint;
                    if (sourceEndpoint != null)
                    {
                        if (Endpoint.AcceptEnvelope(e.Envelope.Id))
                        {
                            asyncResult.Complete(e.Envelope.Messages);
                        }
                    }
                }
            }
        }
    }
}