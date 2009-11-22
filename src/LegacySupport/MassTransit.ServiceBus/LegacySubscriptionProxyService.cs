namespace MassTransit.ServiceBus
{
    //old stuff
    using System;
    using Exceptions;
    using log4net;
    using Messages;
    using Saga;
    using Services.Subscriptions.Messages;
    using OldAddSubscription = MassTransit.ServiceBus.Subscriptions.Messages.AddSubscription;
    using OldRemoveSubscription = MassTransit.ServiceBus.Subscriptions.Messages.RemoveSubscription;
    using OldCacheUpdateRequest = MassTransit.ServiceBus.Subscriptions.Messages.CacheUpdateRequest;
    using OldCacheUpdateResponse = MassTransit.ServiceBus.Subscriptions.Messages.CacheUpdateResponse;
    using OldCancelSubscriptionUpdates = MassTransit.ServiceBus.Subscriptions.Messages.CancelSubscriptionUpdates;

    //new stuff
    using NewSubscriptionRefresh = MassTransit.Services.Subscriptions.Messages.SubscriptionRefresh;
    using NewSubscriptionAdded = MassTransit.Services.Subscriptions.Server.Messages.SubscriptionAdded;
    using NewSubscriptionRemoved = MassTransit.Services.Subscriptions.Server.Messages.SubscriptionRemoved;
    using NewSubscriptionInformation = MassTransit.Services.Subscriptions.Messages.SubscriptionInformation;

    public class LegacySubscriptionProxyService 
    {
        private IEndpoint _subscriptionServiceEndpoint;
        static ILog _log = LogManager.GetLogger(typeof(LegacySubscriptionProxyService));
        private readonly ISagaRepository<LegacySubscriptionClientSaga> _subscriptionClientSagas;
        IEndpointFactory _endpointFactory;
        IServiceBus _bus;
        private UnsubscribeAction _unsubscribeToken = () => false;

        public LegacySubscriptionProxyService(ISagaRepository<LegacySubscriptionClientSaga> subscriptionClientSagas, IEndpointFactory endpointFactory, IServiceBus bus)
        {
            _subscriptionClientSagas = subscriptionClientSagas;
            _endpointFactory = endpointFactory;
            _bus = bus;
        }

        

        //new legacy client
        public void Consume(LegacySubscriptionClientAdded message)
        {
            _subscriptionServiceEndpoint.Send(new AddSubscriptionClient(message.ClientId, message.ControlUri, message.DataUri));
        }

        public void Consume(LegacySubscriptionClientRemoved message)
        {
            _subscriptionServiceEndpoint.Send(new RemoveSubscriptionClient(message.CorrelationId, message.ControlUri, message.DataUri));
        }

        public void Consume(NewSubscriptionAdded message)
        {
            if (_log.IsInfoEnabled)
                _log.InfoFormat("Proxy New to Old Add Subscription: {0}", message.Subscription);

            var add = new OldAddSubscription(message.Subscription.MessageName, message.Subscription.EndpointUri);

            SendToClients(add);
        }

        public void Consume(NewSubscriptionRemoved message)
        {
            if (_log.IsInfoEnabled)
                _log.InfoFormat("Proxy New to OLd Removing Subscription: {0}", message.Subscription);

            var remove = new OldRemoveSubscription(message.Subscription.MessageName, message.Subscription.EndpointUri);

            SendToClients(remove);
        }

        void SendToClients<T>(T message) where T : class
        {
            var sagas = _subscriptionClientSagas.Where(x => x.CurrentState == LegacySubscriptionClientSaga.Active);
            sagas.Each(client =>
                {
                    IEndpoint endpoint = _endpointFactory.GetEndpoint(client.ControlUri);

                    endpoint.Send(message, x => x.SetSourceAddress(_bus.Endpoint.Uri));
                });
        }



        public void Start()
        {
            _log.Info("Legacy Subscription Service Starting");
            _unsubscribeToken += _bus.Subscribe(this);

            _unsubscribeToken += _bus.Subscribe<LegacySubscriptionClientSaga>();

            //the new service
            _subscriptionServiceEndpoint = _endpointFactory.GetEndpoint("msmq://localhost/new_subservice");

            _log.Info("Legacy Subscription Service Started");
        }

        public void Stop()
        {
            _log.Info("Legacy Subscription Service Stopping");

            _unsubscribeToken();

            _log.Info("Legacy Subscription Service Stopped");
        }

        public void Dispose()
        {
            try
            {
                //does this own the bus?
                _bus.Dispose();
                _bus = null;
            }
            catch (Exception ex)
            {
                string message = "Error in shutting down the SubscriptionService: " + ex.Message;
                ShutDownException exp = new ShutDownException(message, ex);
                _log.Error(message, exp);
                throw exp;
            }
        }
    }
}