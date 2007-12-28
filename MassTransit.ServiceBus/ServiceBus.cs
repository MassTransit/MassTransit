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

		private readonly Dictionary<Type, IEndpoint> _messageEndpoints = new Dictionary<Type, IEndpoint>();

        private IEndpoint _endpoint;

        private ISubscriptionStorage _subscriptionStorage;

        public ServiceBus(IEndpoint endpoint, params IEndpoint[] otherEndpoints)
        {
            Check.Parameter(endpoint).WithMessage("endpoint").IsNotNull();

            _endpoint = endpoint;
            _endpoint.EnvelopeReceived += Transport_EnvelopeReceived;

        	_log.DebugFormat("Added event handler for envelope to {0}", _endpoint.Address);
        }

        public ISubscriptionStorage SubscriptionStorage
        {
            get { return _subscriptionStorage; }
            set { _subscriptionStorage = value; }
        }

        #region IServiceBus Members

        public void Publish<T>(params T[] messages) where T : IMessage
        {
            IList<IEndpoint> subscribers = _subscriptionStorage.List<T>();
            if (subscribers.Count > 0)
            {
                Envelope envelope = new Envelope(DefaultEndpoint, messages as IMessage[]);

                foreach (IEndpoint endpoint in subscribers)
                {
                    endpoint.Send(envelope);
                }
            }
        }

        public void Send<T>(IEndpoint endpoint, params T[] messages) where T : IMessage
        {
            Envelope e = new Envelope(DefaultEndpoint, messages as IMessage[]);

            endpoint.Send(e);
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
            //IEndpoint endpoint = _endpointDirectory.Resolve<T>();

            //IEnvelope envelope = _envelopeBuilder.Build(messages);

            //endpoint.Transport.Send(endpoint, envelope);

            throw new NotSupportedException();
        }

        #endregion

        private void Transport_EnvelopeReceived(object sender, EnvelopeReceivedEventArgs e)
        {
			try
			{
				_log.DebugFormat("Envelope {0} Received By {1}", e.Envelope.Id, GetHashCode());

				if (e.Envelope.Messages != null)
				{
					foreach (IMessage message in e.Envelope.Messages)
					{
						_log.DebugFormat("Message Received: {0}", message.GetType());

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
								catch (Exception ex)
								{
									_log.Error("Exception from OnMessageReceived: ", ex);
								}
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				_log.Error("Exception in Transport_EnvelopeReceived: ", ex);
			}
        }
    }
}