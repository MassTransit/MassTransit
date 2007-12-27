using System;
using System.Collections.Generic;
using log4net;
using MassTransit.ServiceBus.Util;

namespace MassTransit.ServiceBus
{
    public class ServiceBus :
        IServiceBus
    {
        private static readonly ILog _log = LogManager.GetLogger("default");
        private readonly Dictionary<Type, IEndpoint> _messageEndpoints = new Dictionary<Type, IEndpoint>();

        private IEndpoint _endpoint;

        private Dictionary<string, IEndpoint> _otherEndpoints;

        private ISubscriptionStorage _subscriptionStorage;

        public ServiceBus(IEndpoint endpoint, params IEndpoint[] otherEndpoints)
        {
            Check.Parameter(endpoint).WithMessage("endpoint").IsNotNull();

            _endpoint = endpoint;
            _endpoint.Transport.EnvelopeReceived += Transport_EnvelopeReceived;

            _otherEndpoints = new Dictionary<string, IEndpoint>();

            foreach (IEndpoint otherEndpoint in otherEndpoints)
            {
                _otherEndpoints.Add(otherEndpoint.Transport.Address, otherEndpoint);
            }
        }

        public ISubscriptionStorage SubscriptionStorage
        {
            get { return _subscriptionStorage; }
            set { _subscriptionStorage = value; }
        }

        #region IServiceBus Members

        public void Send<T>(params T[] messages) where T : IMessage
        {
            // for now, we'll only send to ourself
            IEndpoint endpoint = DefaultEndpoint;

            Envelope envelope = new Envelope(endpoint, messages as IMessage[]);

            endpoint.Transport.Send(envelope);
        }

        public void Publish<T>(params T[] messages) where T : IMessage
        {
            IList<IEndpoint> subscribers = _subscriptionStorage.List<T>();
            if (subscribers.Count > 0)
            {
                Envelope envelope = new Envelope(DefaultEndpoint, messages as IMessage[]);

                foreach (IEndpoint endpoint in subscribers)
                {
                    endpoint.Transport.Send(envelope);
                }
            }
        }

        public void Send<T>(IEndpoint endpoint, params T[] messages) where T : IMessage
        {
            Envelope e = new Envelope(DefaultEndpoint, messages as IMessage[]);

            endpoint.Transport.Send(e);
        }

        public IEndpoint DefaultEndpoint
        {
            get { return _endpoint; }
            set { _endpoint = value; }
        }

        public IMessageEndpoint<T> Endpoint<T>() where T : IMessage
        {
            IEndpoint result;
            _messageEndpoints.TryGetValue(typeof (T), out result);

            if (result == null)
            {
                _messageEndpoints[typeof (T)] = new MessageEndpoint<T>(DefaultEndpoint);

                result = _messageEndpoints[typeof (T)];

                _subscriptionStorage.Add(typeof (T), DefaultEndpoint);
            }

            return result as IMessageEndpoint<T>;
        }

        public IServiceBusAsyncResult Request<T>(params T[] messages) where T : IMessage
        {
            //IEndpoint endpoint = _endpointDirectory.Get<T>();

            //IEnvelope envelope = _envelopeBuilder.Build(messages);

            //endpoint.Transport.Send(endpoint, envelope);

            throw new NotSupportedException();
        }

        #endregion

        private void Transport_EnvelopeReceived(object sender, EnvelopeReceivedEventArgs e)
        {
            if (e.Envelope.Messages != null)
            {
                foreach (IMessage message in e.Envelope.Messages)
                {
                    if (_messageEndpoints.ContainsKey(message.GetType()))
                    {
                        IMessageEndpointReceive receivingEndpoint =
                            _messageEndpoints[message.GetType()] as IMessageEndpointReceive;
                        if (receivingEndpoint != null)
                        {
                            try
                            {
                                receivingEndpoint.OnMessageReceived(e.Envelope, message);
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
        }
    }
}