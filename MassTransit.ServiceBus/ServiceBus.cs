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
    public class ServiceBus :
        IServiceBus,
        IEnvelopeConsumer
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Dictionary<Type, INotifyMessageConsumer> _consumers =
            new Dictionary<Type, INotifyMessageConsumer>();

        private readonly object _consumersLock = new object();

        private readonly CorrelatedMessageController _correlatedMessageController = new CorrelatedMessageController();
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

            _receiver = MessageReceiverFactory.Create(_endpoint);
            if (_receiver != null)
            {
                _receiver.Subscribe(this);
            }

            _sender = MessageSenderFactory.Create(_endpoint);


            // TODO find this a new home in the receiver
            //if (_log.IsDebugEnabled)
            //    _log.DebugFormat("Added event handler for envelope to {0}", _endpoint.Address);
        }

        public ISubscriptionStorage SubscriptionStorage
        {
            get { return _subscriptionStorage; }
        }

        #region IEnvelopeConsumer Members

        public bool MeetsCriteria(IEnvelope envelope)
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
                                INotifyMessageConsumer receivingConsumer =
                                    _consumers[message.GetType()];

                                if (receivingConsumer.MeetsCriteria(message))
                                    return true;
                            }
                        }
                    }
                }
                else
                {
                    if (_correlatedMessageController.Match(envelope.CorrelationId).Found)
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

                lock (_correlatedMessageController)
                {
                    if (_correlatedMessageController.Process(envelope).WasHandled)
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

        public void Dispose()
        {
            _subscriptionStorage.Dispose();

            _consumers.Clear();

            _receiver.Dispose();

            _sender.Dispose();

            _endpoint.Dispose();
        }

        public void Publish<T>(params T[] messages) where T : IMessage
        {
            IList<Uri> subscribers = _subscriptionStorage.List();
            if (subscribers.Count > 0)
            {
                IEnvelope envelope = new Envelope(_endpoint, messages as IMessage[]);

                foreach (Uri subscribersEndpoint in subscribers)
                {
                    IMessageSender send = MessageSenderFactory.Create(subscribersEndpoint);
                    send.Send(envelope);
                }
            }
        }

        public void Send<T>(IEndpoint destinationEndpoint, params T[] messages) where T : IMessage
        {
            IEnvelope envelope = new Envelope(_endpoint, messages as IMessage[]);

            IMessageSender send = MessageSenderFactory.Create(destinationEndpoint);
            send.Send(envelope);
        }

        public IEndpoint Endpoint
        {
            get { return _endpoint; }
        }

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

        public void Subscribe<T>(MessageReceivedCallback<T> callback) where T : IMessage
        {
            Subscribe(callback, null);
        }

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

        public IServiceBusAsyncResult Request<T>(IEndpoint destinationEndpoint, params T[] messages) where T : IMessage
        {
            IEnvelope envelope = new Envelope(_endpoint, messages as IMessage[]);

            IMessageSender send = MessageSenderFactory.Create(destinationEndpoint);
            lock (_correlatedMessageController)
            {            
                send.Send(envelope);
 
                return _correlatedMessageController.Track(envelope.Id);
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
    }
}