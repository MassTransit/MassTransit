namespace MassTransit.ServiceBus.SubscriptionsManager
{
    using System;
    using System.Collections.Generic;
    using log4net;
    using MassTransit.ServiceBus.Subscriptions.Messages;
    using Subscriptions;

    public class SubscriptionService :
        IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (SubscriptionService));
        private readonly IServiceBus _bus;
        private readonly ISubscriptionStorage _cache;
        private readonly ISubscriptionStorage _repository;

        public SubscriptionService(IServiceBus bus, ISubscriptionStorage subscriptionCache, ISubscriptionStorage subscriptionRepository)
        {
            _bus = bus;
            _cache = subscriptionCache;
            _repository = subscriptionRepository;
        }

        #region IDisposable Members

        public void Dispose()
        {
            _bus.Dispose();
            _cache.Dispose();
            _repository.Dispose();
        }

        #endregion

        public void Start(string[] args)
        {
            foreach (Subscription sub in _repository.List())
            {
                _cache.Add(sub.MessageName, sub.Address);
            }

            _bus.Subscribe<CacheUpdateRequest>(HandleCacheUpdateRequest);
            _bus.Subscribe<SubscriptionChange>(HandleSubscriptionChange);
        }

        public void Stop()
        {
            _bus.Unsubscribe<CacheUpdateRequest>(HandleCacheUpdateRequest);
            _bus.Unsubscribe<SubscriptionChange>(HandleSubscriptionChange);
        }

        public void HandleSubscriptionChange(IMessageContext<SubscriptionChange> ctx)
        {
            try
            {
                // TODO RegisterSenderForUpdates(ctx.Envelope); I DON"T THINK SO

                switch (ctx.Message.ChangeType)
                {
                    case SubscriptionChangeType.Add:
                        _cache.Add(ctx.Message.Subscription.MessageName, ctx.Message.Subscription.Address);
                        _repository.Add(ctx.Message.Subscription.MessageName, ctx.Message.Subscription.Address);
                        break;

                    case SubscriptionChangeType.Remove:
                        _cache.Remove(ctx.Message.Subscription.MessageName, ctx.Message.Subscription.Address);
                        _repository.Remove(ctx.Message.Subscription.MessageName, ctx.Message.Subscription.Address);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException("ChangeType");
                }

                // this.Publish(ctx.Message);
            }
            catch (Exception ex)
            {
                _log.Error("Exception handling subscription change", ex);
            }
        }

        public void HandleCacheUpdateRequest(IMessageContext<CacheUpdateRequest> ctx)
        {
            try
            {
                // TODO RegisterSenderForUpdates(ctx.Envelope);

                IList<Subscription> subscriptions = _cache.List();

                CacheUpdateResponse response = new CacheUpdateResponse(subscriptions);

                ctx.Reply(response);
            }
            catch (Exception ex)
            {
                _log.Error("Exception handling cache update request", ex);
            }
        }
    }
}