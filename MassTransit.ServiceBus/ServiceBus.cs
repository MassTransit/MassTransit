/// Copyright 2007-2008 The Apache Software Foundation.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
/// this file except in compliance with the License. You may obtain a copy of the 
/// License at 
/// 
///   http://www.apache.org/licenses/LICENSE-2.0 
/// 
/// Unless required by applicable law or agreed to in writing, software distributed 
/// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
/// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
/// specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;
using MassTransit.ServiceBus.Util;

namespace MassTransit.ServiceBus
{
    /// <summary>
    /// A service bus is used to attach message handlers (services) to endpoints, as well as 
    /// communicate with other service bus instances in a distributed application
    /// </summary>
    public class ServiceBus :
        IServiceBus,
        IEnvelopeConsumer
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Dictionary<Type, IMessageConsumer> _consumers =
            new Dictionary<Type, IMessageConsumer>();

        private readonly object _consumersLock = new object();

        private readonly AsyncReplyDispatcher _AsyncReplyDispatcher = new AsyncReplyDispatcher();
        private readonly IEndpoint _endpoint;
        private readonly IMessageReceiver _receiver;
        private readonly IMessageSender _sender;
        private readonly ISubscriptionStorage _subscriptionStorage;
        private IEndpoint _poisonEndpoint;

        public ServiceBus(IEndpoint endpoint, ISubscriptionStorage subscriptionStorage)
        {
            Check.Parameter(endpoint).WithMessage("endpoint").IsNotNull();
            Check.Parameter(subscriptionStorage).WithMessage("subscriptionStorage").IsNotNull();

            _endpoint = endpoint;
            _subscriptionStorage = subscriptionStorage;

            _receiver = MessageReceiver.Using(_endpoint);
            _receiver.Subscribe(this);

            _sender = MessageSender.Using(_endpoint);
        }

        public ISubscriptionStorage SubscriptionStorage
        {
            get { return _subscriptionStorage; }
        }

        #region IEnvelopeConsumer Members

        /// <summary>
        /// Called when a message is available from the endpoint. If the consumer returns true, the message
        /// will be removed from the endpoint and delivered to the consumer
        /// </summary>
        /// <param name="envelope">The message envelope available</param>
        /// <returns>True is the consumer will handle the message, false if it should be ignored</returns>
        public bool IsHandled(IEnvelope envelope)
        {
            try
            {
                if (envelope.CorrelationId == MessageId.Empty)
                {
                    if (envelope.Messages != null)
                    {
                        foreach (IMessage message in envelope.Messages)
                        {
                            if (_consumers.ContainsKey(message.GetType()))
                            {
                                IMessageConsumer receivingConsumer =
                                    _consumers[message.GetType()];

                                if (receivingConsumer.IsHandled(message))
                                    return true;
                            }
                        }
                    }
                }
                else
                {
                    if (_AsyncReplyDispatcher.Exists(envelope.CorrelationId))
                        return true;
                }
            }
            catch (Exception ex)
            {
                _log.Error("Exception in Endpoint_EnvelopeReceived: ", ex);
            }

            return false;
        }

        /// <summary>
        /// Delivers the message envelope to the consumer
        /// </summary>
        /// <param name="envelope">The message envelope</param>
        public void Deliver(IEnvelope envelope)
        {
            try
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Envelope {0} Received By {1}", envelope.Id, GetHashCode());

                lock (_AsyncReplyDispatcher)
                {
                    if (_AsyncReplyDispatcher.Complete(envelope))
                        return;
                }

                MessageDoesntHaveCorrelationId(envelope);
            }
            catch (Exception ex)
            {
                _log.Error("Exception in Endpoint_EnvelopeReceived: ", ex);
            }
        }

        #endregion

        #region IServiceBus Members

        ///<summary>
        ///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        ///</summary>
        ///<filterpriority>2</filterpriority>
        public void Dispose()
        {
            _subscriptionStorage.Dispose();

            _consumers.Clear();

            _receiver.Dispose();

            _sender.Dispose();

            _endpoint.Dispose();
        }

        /// <summary>
        /// Publishes a message to all subscribed consumers for the message type
        /// </summary>
        /// <typeparam name="T">The type of the message</typeparam>
        /// <param name="messages">The messages to be published</param>
        public void Publish<T>(params T[] messages) where T : IMessage
        {
            IList<Uri> subscribers = _subscriptionStorage.List();
            if (subscribers.Count > 0)
            {
                IEnvelope envelope = new Envelope(_endpoint, messages as IMessage[]);

                foreach (Uri subscribersEndpoint in subscribers)
                {
                    IMessageSender send = MessageSender.Using(subscribersEndpoint);
                    send.Send(envelope);
                }
            }
        }

        /// <summary>
        /// Sends a list of messages to the specified destination
        /// </summary>
        /// <param name="destinationEndpoint">The destination for the message</param>
        /// <param name="messages">The list of messages</param>
        public void Send<T>(IEndpoint destinationEndpoint, params T[] messages) where T : IMessage
        {
            IEnvelope envelope = new Envelope(_endpoint, messages as IMessage[]);

            IMessageSender send = MessageSender.Using(destinationEndpoint);
            send.Send(envelope);
        }

        /// <summary>
        /// The endpoint associated with this instance
        /// </summary>
        public IEndpoint Endpoint
        {
            get { return _endpoint; }
        }

        /// <summary>
        /// The poison endpoint associated with this instance where exception messages are sent
        /// </summary>
        public IEndpoint PoisonEndpoint
        {
            get
            {
                if (_poisonEndpoint == null)
                {
                    _poisonEndpoint = new MessageQueueEndpoint(_endpoint.Uri + "_poison");
                }

                return _poisonEndpoint;
            }
            set { _poisonEndpoint = value; }
        }

        /// <summary>
        /// Adds a message handler to the service bus for handling a specific type of message
        /// </summary>
        /// <typeparam name="T">The message type to handle, often inferred from the callback specified</typeparam>
        /// <param name="callback">The callback to invoke when messages of the specified type arrive on the service bus</param>
        public void Subscribe<T>(MessageReceivedCallback<T> callback) where T : IMessage
        {
            Subscribe(callback, null);
        }

        /// <summary>
        /// Adds a message handler to the service bus for handling a specific type of message
        /// </summary>
        /// <typeparam name="T">The message type to handle, often inferred from the callback specified</typeparam>
        /// <param name="callback">The callback to invoke when messages of the specified type arrive on the service bus</param>
        /// <param name="condition">A condition predicate to filter which messages are handled by the callback</param>
        public void Subscribe<T>(MessageReceivedCallback<T> callback, Predicate<T> condition) where T : IMessage
        {
            lock (_consumersLock)
            {
                if (!_consumers.ContainsKey(typeof (T)))
                {
                    _consumers[typeof (T)] = new MessageConsumer<T>();
                    _subscriptionStorage.Add(typeof (T).FullName, Endpoint.Uri);
                }

                ((IMessageConsumer<T>) _consumers[typeof (T)]).Subscribe(callback, condition);
            }
        }

        /// <summary>
        /// Submits a request message to the default destination for the message type
        /// </summary>
        /// <typeparam name="T">The type of message</typeparam>
        /// <param name="destinationEndpoint">The destination for the message</param>
        /// <param name="messages">The messages to be sent</param>
        /// <returns>An IAsyncResult that can be used to wait for the response</returns>
        public IServiceBusAsyncResult Request<T>(IEndpoint destinationEndpoint, params T[] messages) where T : IMessage
        {
            IEnvelope envelope = new Envelope(_endpoint, messages as IMessage[]);

            IMessageSender send = MessageSender.Using(destinationEndpoint);
            lock (_AsyncReplyDispatcher)
            {            
                send.Send(envelope);
 
                return _AsyncReplyDispatcher.Track(envelope.Id);
            }
        }

        #endregion

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
                        IMessageConsumer receivingConsumer =
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
    }
}