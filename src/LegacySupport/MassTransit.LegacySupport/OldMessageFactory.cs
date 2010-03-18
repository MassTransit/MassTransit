namespace MassTransit.LegacySupport
{
    using System;
    using AutoMapper;
    using ServiceBus.Subscriptions;
    using ServiceBus.Subscriptions.Messages;

    public class OldMessageFactory
    {
        readonly Type _weakCacheUpdateResponseType;
        readonly Type _weakSubscriptionType;
        readonly Type _weakAddSubscriptionType;
        readonly Type _weakCancelSubscriptionUpdatesType;
        readonly Type _weakRemoveSubscriptionType;
        readonly Type _weakCacheUpdateRequestType;

        public OldMessageFactory()
        {
            _weakCacheUpdateResponseType = Type.GetType("MassTransit.ServiceBus.Subscriptions.Messages.CacheUpdateResponse, MassTransit.ServiceBus, Version=0.2.2133.0, Culture=neutral, PublicKeyToken=null");
            _weakCacheUpdateRequestType = Type.GetType("MassTransit.ServiceBus.Subscriptions.Messages.CacheUpdateRequest, MassTransit.ServiceBus, Version=0.2.2133.0, Culture=neutral, PublicKeyToken=null");
            _weakSubscriptionType = Type.GetType("MassTransit.ServiceBus.Subscriptions.Subscription, MassTransit.ServiceBus, Version=0.2.2133.0, Culture=neutral, PublicKeyToken=null");
            _weakAddSubscriptionType = Type.GetType("MassTransit.ServiceBus.Subscriptions.Messages.AddSubscription, MassTransit.ServiceBus, Version=0.2.2133.0, Culture=neutral, PublicKeyToken=null");
            _weakRemoveSubscriptionType = Type.GetType("MassTransit.ServiceBus.Subscriptions.Messages.RemoveSubscription, MassTransit.ServiceBus, Version=0.2.2133.0, Culture=neutral, PublicKeyToken=null");
            _weakCancelSubscriptionUpdatesType = Type.GetType("MassTransit.ServiceBus.Subscriptions.Messages.CancelSubscriptionUpdates, MassTransit.ServiceBus, Version=0.2.2133.0, Culture=neutral, PublicKeyToken=null");
            
            Mapper.CreateMap(_weakSubscriptionType, typeof(Subscription));
            Mapper.CreateMap(_weakCacheUpdateResponseType, typeof(CacheUpdateResponse));
            Mapper.CreateMap(_weakCacheUpdateRequestType, typeof(CacheUpdateRequest));
            Mapper.CreateMap(_weakAddSubscriptionType, typeof(AddSubscription));
            Mapper.CreateMap(_weakRemoveSubscriptionType, typeof(RemoveSubscription));
            Mapper.CreateMap(_weakCancelSubscriptionUpdatesType, typeof(CancelSubscriptionUpdates));

            Mapper.CreateMap(typeof(Subscription), _weakSubscriptionType);
            Mapper.CreateMap(typeof(CacheUpdateResponse), _weakCacheUpdateResponseType);
            Mapper.CreateMap(typeof(CacheUpdateRequest), _weakCacheUpdateRequestType);
            Mapper.CreateMap(typeof(AddSubscription), _weakAddSubscriptionType);
            Mapper.CreateMap(typeof(RemoveSubscription), _weakRemoveSubscriptionType);
            Mapper.CreateMap(typeof(CancelSubscriptionUpdates), _weakCancelSubscriptionUpdatesType);
        }

        public CacheUpdateResponse ConvertToNewCacheUpdateResponse(object old)
        {
            return (CacheUpdateResponse)Mapper.Map(old, _weakCacheUpdateResponseType, typeof(CacheUpdateResponse));
        }
        public object ConvertToOldCacheUpdateResponse(CacheUpdateResponse response)
        {
            return Mapper.Map(response, typeof(CacheUpdateResponse), _weakCacheUpdateResponseType);
        }

        public object ConvertToOldAddSubscription(AddSubscription subscription)
        {
            return Mapper.Map(subscription, typeof (AddSubscription), _weakAddSubscriptionType);
        }

        public AddSubscription ConvertToNewAddSubscription(object o)
        {
            return (AddSubscription)Mapper.Map(o,  _weakAddSubscriptionType, typeof(AddSubscription));
        }

        public object ConvertToOldCancelSubscriptionUpdates(CancelSubscriptionUpdates updates)
        {
            return Mapper.Map(updates, typeof (CancelSubscriptionUpdates), _weakCancelSubscriptionUpdatesType);
        }

        public CancelSubscriptionUpdates ConvertToNewCancelSubscriptionUpdates(object updates)
        {
            return (CancelSubscriptionUpdates)Mapper.Map(updates, _weakCancelSubscriptionUpdatesType, typeof (CancelSubscriptionUpdates));
        }

        public object ConvertToOldRemoveSubscription(RemoveSubscription subscription)
        {
            return Mapper.Map(subscription, typeof (RemoveSubscription), _weakRemoveSubscriptionType);
        }

        public RemoveSubscription ConvertToNewRemoveSubscription(object subscription)
        {
            return (RemoveSubscription)Mapper.Map(subscription, _weakRemoveSubscriptionType, typeof(RemoveSubscription));
        }

        public object ConvertToOldCacheUpdateRequest(CacheUpdateRequest request)
        {
            return Mapper.Map(request, typeof (CacheUpdateRequest), _weakCacheUpdateRequestType);
        }

        public CacheUpdateRequest ConvertToNewCacheUpdateRequest(object request)
        {
            return (CacheUpdateRequest)Mapper.Map(request, _weakCacheUpdateRequestType, typeof(CacheUpdateRequest));
        }
    }
}