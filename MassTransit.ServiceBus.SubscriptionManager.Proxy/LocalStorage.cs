namespace MassTransit.ServiceBus.SubscriptionManager.Proxy
{
    using System;
    using System.Collections.Generic;
    using log4net;
    using MassTransit.ServiceBus.SubscriptionsManager.Messages;
    using Subscriptions;

    public class LocalStorage : ISubscriptionStorage
    {
        private IServiceBus _bus;
        private IEndpoint _wellKnownSubscriptionManagerEndpoint;
        private readonly Dictionary<Type, List<SubscriptionCacheEntry>> _messageTypeSubscriptions =
            new Dictionary<Type, List<SubscriptionCacheEntry>>();
        private static readonly ILog _log = LogManager.GetLogger(typeof(LocalStorage));

        public LocalStorage(IServiceBus bus, IEndpoint wellKnownSubscriptionManagerEndpoint)
        {
            _bus = bus;
            _bus.Subscribe<CacheUpdateResponse>(ReactToCacheUpdateResponse);
            _wellKnownSubscriptionManagerEndpoint = wellKnownSubscriptionManagerEndpoint;
        }

        public void Initialize()
        {
            _bus.Send(_wellKnownSubscriptionManagerEndpoint, new RequestCacheUpdate());
        }

        public IList<IEndpoint> List<T>(params T[] messages) where T : IMessage
        {
            List<IEndpoint> result = new List<IEndpoint>();
            if (_messageTypeSubscriptions.ContainsKey(typeof(T)))
            {
                _messageTypeSubscriptions[typeof(T)].ForEach(
                    delegate(SubscriptionCacheEntry entry) { result.Add(entry.Endpoint); });
            }

            return result;
        }
        public void Add(Type messageType, IEndpoint endpoint)
        {
            InternalAdd(messageType, endpoint);
            if(_log.IsInfoEnabled)
                _log.InfoFormat("Sending Subscription Update ({0}, {1}) to Master Repository", messageType, endpoint.Address);
            _bus.Send(_wellKnownSubscriptionManagerEndpoint, new SubscriptionMessage(messageType, endpoint.Address, SubscriptionMessage.SubscriptionChangeType.Add));
        }
        public void Remove(Type messageType, IEndpoint endpoint)
        {
            InternalRemove(messageType, endpoint);
            if (_log.IsInfoEnabled)
                _log.InfoFormat("Sending Subscription Update ({0}, {1}) to Master Repository", messageType, endpoint.Address);
            _bus.Send(_wellKnownSubscriptionManagerEndpoint, new SubscriptionMessage(messageType, endpoint.Address, SubscriptionMessage.SubscriptionChangeType.Remove));
        }

        public void Dispose()
        {
            _wellKnownSubscriptionManagerEndpoint.Dispose();
            _messageTypeSubscriptions.Clear();
        }

        private void ReactToCacheUpdateResponse(MessageContext<CacheUpdateResponse> cxt)
        {
            
            cxt.Message.Subscriptions.ForEach(delegate (SubscriptionMessage msg)
                                                  {
                                                      switch(msg.ChangeType)
                                                      {
                                                          case SubscriptionMessage.SubscriptionChangeType.Add:
                                                              InternalAdd(msg.MessageType, new MessageQueueEndpoint(msg.Address));
                                                              break;
                                                          case SubscriptionMessage.SubscriptionChangeType.Remove:
                                                              //TODO: Infinite Messages?
                                                              InternalRemove(msg.MessageType, new MessageQueueEndpoint(msg.Address));
                                                              break;
                                                          default:
                                                              throw new ArgumentOutOfRangeException();
                                                      }
                                                  });
            if (_log.IsInfoEnabled)
                _log.InfoFormat("Cache Update Complete");
        }
        
        private void InternalRemove(Type messageType, IEndpoint endpoint)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Removing Local Subscription {0} : {1}", messageType, endpoint.Address);

            lock (this)
            {
                if (_messageTypeSubscriptions.ContainsKey(messageType))
                {
                    SubscriptionCacheEntry entry = new SubscriptionCacheEntry(endpoint);

                    if (_messageTypeSubscriptions[messageType].Contains(entry))
                    {
                        if (_log.IsDebugEnabled)
                            _log.DebugFormat("Removing local subscription entry for endpoint {0} on {1}", endpoint.Address,
                                             GetHashCode());
                        _messageTypeSubscriptions[messageType].Remove(entry);
                    }

                    if (_messageTypeSubscriptions[messageType].Count == 0)
                    {
                        if (_log.IsDebugEnabled)
                            _log.DebugFormat("Removing local subscription list for type {0} on {1}", messageType.ToString(), GetHashCode());
                        _messageTypeSubscriptions.Remove(messageType);
                    }
                }   
            }
        }
        private void InternalAdd(Type messageType, IEndpoint endpoint)
        {
            lock (this)
            {
                if (!_messageTypeSubscriptions.ContainsKey(messageType))
                {
                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Adding new local subscription list for type {0} on {1}", messageType.ToString(), GetHashCode());
                    _messageTypeSubscriptions.Add(messageType, new List<SubscriptionCacheEntry>());
                }

                SubscriptionCacheEntry entry = new SubscriptionCacheEntry(endpoint);

                if (!_messageTypeSubscriptions[messageType].Contains(entry))
                {
                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Adding new local subscription entry for endpoint {0} on {1}", endpoint.Address,
                                         GetHashCode());
                    _messageTypeSubscriptions[messageType].Add(entry);
                }
            }
        }
    }
}