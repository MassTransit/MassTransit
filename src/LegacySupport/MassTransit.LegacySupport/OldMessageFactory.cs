namespace MassTransit.LegacySupport
{
    using System;
    using AutoMapper;
    using ProxyMessages;

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
            Mapper.CreateMap(_weakCacheUpdateResponseType, typeof(OldCacheUpdateResponse));
            Mapper.CreateMap(_weakCacheUpdateRequestType, typeof(OldCacheUpdateRequest));
            Mapper.CreateMap(_weakAddSubscriptionType, typeof(OldAddSubscription));
            Mapper.CreateMap(_weakRemoveSubscriptionType, typeof(OldRemoveSubscription));
            Mapper.CreateMap(_weakCancelSubscriptionUpdatesType, typeof(OldCancelSubscriptionUpdates));

            Mapper.CreateMap(typeof(Subscription), _weakSubscriptionType);
            Mapper.CreateMap(typeof(OldCacheUpdateResponse), _weakCacheUpdateResponseType);
            Mapper.CreateMap(typeof(OldCacheUpdateRequest), _weakCacheUpdateRequestType);
            Mapper.CreateMap(typeof(OldAddSubscription), _weakAddSubscriptionType);
            Mapper.CreateMap(typeof(OldRemoveSubscription), _weakRemoveSubscriptionType);
            Mapper.CreateMap(typeof(OldCancelSubscriptionUpdates), _weakCancelSubscriptionUpdatesType);
        }

        public OldCacheUpdateResponse ConvertToNewCacheUpdateResponse(object old)
        {
            return (OldCacheUpdateResponse)Mapper.Map(old, _weakCacheUpdateResponseType, typeof(OldCacheUpdateResponse));
        }
        public object ConvertToOldCacheUpdateResponse(OldCacheUpdateResponse response)
        {
            return Mapper.Map(response, typeof(OldCacheUpdateResponse), _weakCacheUpdateResponseType);
        }

        public object ConvertToOldAddSubscription(OldAddSubscription subscription)
        {
            return Mapper.Map(subscription, typeof (OldAddSubscription), _weakAddSubscriptionType);
        }

        public OldAddSubscription ConvertToNewAddSubscription(object o)
        {
            return (OldAddSubscription)Mapper.Map(o,  _weakAddSubscriptionType, typeof(OldAddSubscription));
        }

        public object ConvertToOldCancelSubscriptionUpdates(OldCancelSubscriptionUpdates updates)
        {
            return Mapper.Map(updates, typeof (OldCancelSubscriptionUpdates), _weakCancelSubscriptionUpdatesType);
        }

        public OldCancelSubscriptionUpdates ConvertToNewCancelSubscriptionUpdates(object updates)
        {
            return (OldCancelSubscriptionUpdates)Mapper.Map(updates, _weakCancelSubscriptionUpdatesType, typeof (OldCancelSubscriptionUpdates));
        }

        public object ConvertToOldRemoveSubscription(OldRemoveSubscription subscription)
        {
            return Mapper.Map(subscription, typeof (OldRemoveSubscription), _weakRemoveSubscriptionType);
        }

        public OldRemoveSubscription ConvertToNewRemoveSubscription(object subscription)
        {
            return (OldRemoveSubscription)Mapper.Map(subscription, _weakRemoveSubscriptionType, typeof(OldRemoveSubscription));
        }

        public object ConvertToOldCacheUpdateRequest(OldCacheUpdateRequest request)
        {
            return Mapper.Map(request, typeof (OldCacheUpdateRequest), _weakCacheUpdateRequestType);
        }

        public OldCacheUpdateRequest ConvertToNewCacheUpdateRequest(object request)
        {
            return (OldCacheUpdateRequest)Mapper.Map(request, _weakCacheUpdateRequestType, typeof(OldCacheUpdateRequest));
        }
    }
}