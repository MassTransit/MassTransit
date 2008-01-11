namespace MassTransit.ServiceBus.Subscriptions
{
    using System;
    using System.Collections.Generic;
    using log4net;
    using Messages;

    public class LocalSubscriptionCache : 
        ISubscriptionStorage
    {
        private IEndpoint _wellKnownSubscriptionManagerEndpoint;
        private readonly Dictionary<string, List<SubscriptionCacheEntry>> _messageTypeSubscriptions =
            new Dictionary<string, List<SubscriptionCacheEntry>>();
        private static readonly ILog _log = LogManager.GetLogger(typeof(LocalSubscriptionCache));

        // just a shared local cache
        public LocalSubscriptionCache()
        {
            
        }

        /// <summary>
        /// You must call Initialize evertime
        /// You must call RegisterWithBus in order to get distributed subscription management
        /// </summary>
        /// <param name="wellKnownSubscriptionManagerEndpoint"></param>
        public LocalSubscriptionCache(IEndpoint wellKnownSubscriptionManagerEndpoint)
        {
            _wellKnownSubscriptionManagerEndpoint = wellKnownSubscriptionManagerEndpoint;
        }

        public event EventHandler<SubscriptionChangedEventArgs> SubscriptionChanged;

        public IList<Uri> List(string messageName)
        {
            List<Uri> result = new List<Uri>();
            if (_messageTypeSubscriptions.ContainsKey(messageName))
            {
                _messageTypeSubscriptions[messageName].ForEach(
                    delegate(SubscriptionCacheEntry entry) { result.Add(entry.Endpoint); });
            }

            return result;
        }


        public IList<Uri> List()
        {
            List<Uri> result = new List<Uri>();
            
            foreach (List<SubscriptionCacheEntry> list in _messageTypeSubscriptions.Values)
            {
                list.ForEach(delegate(SubscriptionCacheEntry e) { result.Add(e.Endpoint);});
            }

            return result;
        }

        public void Add(string messageName, Uri endpoint)
        {
            InternalAdd(messageName, endpoint);
            if(_log.IsInfoEnabled)
                _log.InfoFormat("Sending Subscription Update ({0}, {1}) to Master Repository", messageName, endpoint);
            OnChange(new SubscriptionChange(messageName, endpoint, SubscriptionChange.SubscriptionChangeType.Add));
        }
        public void Remove(string messageName, Uri endpoint)
        {
            InternalRemove(messageName, endpoint);
            if (_log.IsInfoEnabled)
                _log.InfoFormat("Sending Subscription Update ({0}, {1}) to Master Repository", messageName, endpoint);
			OnChange(new SubscriptionChange(messageName, endpoint, SubscriptionChange.SubscriptionChangeType.Remove));
        }

        public void Dispose()
        {
            if(_wellKnownSubscriptionManagerEndpoint != null)
                _wellKnownSubscriptionManagerEndpoint.Dispose();

            _messageTypeSubscriptions.Clear();
        }

        public void ReactToCacheUpdateResponse(MessageContext<CacheUpdateResponse> cxt)
        {
            
            cxt.Message.Subscriptions.ForEach(delegate (SubscriptionChange msg)
                                                  {
                                                      switch(msg.ChangeType)
                                                      {
                                                          case SubscriptionChange.SubscriptionChangeType.Add:
                                                              InternalAdd(msg.MessageName, msg.Address);
                                                              break;
                                                          case SubscriptionChange.SubscriptionChangeType.Remove:
                                                              InternalRemove(msg.MessageName, msg.Address);
                                                              break;
                                                          default:
                                                              throw new ArgumentOutOfRangeException();
                                                      }
                                                  });
            if (_log.IsInfoEnabled)
                _log.InfoFormat("Cache Update Complete");
        }
        
        private void InternalRemove(string messageName, Uri endpoint)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Removing Local Subscription {0} : {1}", messageName, endpoint);

            lock (this)
            {
                if (_messageTypeSubscriptions.ContainsKey(messageName))
                {
                    SubscriptionCacheEntry entry = new SubscriptionCacheEntry(endpoint);

                    if (_messageTypeSubscriptions[messageName].Contains(entry))
                    {
                        if (_log.IsDebugEnabled)
                            _log.DebugFormat("Removing local subscription entry for endpoint {0} on {1}", endpoint,
                                             GetHashCode());
                        _messageTypeSubscriptions[messageName].Remove(entry);
                    }

                    if (_messageTypeSubscriptions[messageName].Count == 0)
                    {
                        if (_log.IsDebugEnabled)
                            _log.DebugFormat("Removing local subscription list for type {0} on {1}", messageName, GetHashCode());
                        _messageTypeSubscriptions.Remove(messageName);
                    }
                }   
            }
        }
        private void InternalAdd(string messageName, Uri endpoint)
        {
            lock (this)
            {
                if (!_messageTypeSubscriptions.ContainsKey(messageName))
                {
                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Adding new local subscription list for type {0} on {1}", messageName, GetHashCode());
                    _messageTypeSubscriptions.Add(messageName, new List<SubscriptionCacheEntry>());
                }

                SubscriptionCacheEntry entry = new SubscriptionCacheEntry(endpoint);

                if (!_messageTypeSubscriptions[messageName].Contains(entry))
                {
                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Adding new local subscription entry for endpoint {0} on {1}", endpoint,
                                         GetHashCode());
                    _messageTypeSubscriptions[messageName].Add(entry);
                }
            }
        }

        protected void OnChange(SubscriptionChange change)
        {
            EventHandler<SubscriptionChangedEventArgs> handler = this.SubscriptionChanged;
            if (handler != null)
            {
                handler(this, new SubscriptionChangedEventArgs(change));
            }
        }
    }
}