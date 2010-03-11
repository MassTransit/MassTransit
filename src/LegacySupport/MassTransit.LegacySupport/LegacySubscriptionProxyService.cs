namespace MassTransit.LegacySupport
{
    using System;
    using System.Reflection;
    using Exceptions;
    using log4net;
    using Messages;
    using Saga;
    using ServiceBus.Subscriptions.Messages;
    using Services.Subscriptions.Messages;
    using Services.Subscriptions.Server.Messages;
    using AddSubscription=MassTransit.ServiceBus.Subscriptions.Messages.AddSubscription;
    using RemoveSubscription=MassTransit.ServiceBus.Subscriptions.Messages.RemoveSubscription;

    public class LegacySubscriptionProxyService 
    {
        private IEndpoint _subscriptionServiceEndpoint;
        static ILog _log = LogManager.GetLogger(typeof(LegacySubscriptionProxyService));
        private readonly ISagaRepository<LegacySubscriptionClientSaga> _subscriptionClientSagas;
        IEndpointFactory _endpointFactory;
        IServiceBus _bus;
        private UnsubscribeAction _unsubscribeToken = () => false;

        public static void SetupAssemblyRedirectForOldMessages()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
        }

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

        public void Consume(SubscriptionAdded message)
        {
            _log.InfoFormat("Proxy New to Old Add Subscription: {0}", message.Subscription);

            var add = new AddSubscription(message.Subscription.MessageName, message.Subscription.EndpointUri);

            SendToClients(add);
        }

        public void Consume(SubscriptionRemoved message)
        {
            _log.InfoFormat("Proxy New to OLd Removing Subscription: {0}", message.Subscription);

            var remove = new RemoveSubscription(message.Subscription.MessageName, message.Subscription.EndpointUri);

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
            _subscriptionServiceEndpoint = _endpointFactory.GetEndpoint("msmq://localhost/mt_subscriptions");

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

        private static Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name != "MassTransit.ServiceBus, Version=0.2.2133.0, Culture=neutral, PublicKeyToken=null")
                return null;
            var assembly = Assembly.GetAssembly(typeof(LegacySubscriptionProxyService));
            var resourceStream = assembly.GetManifestResourceStream("MassTransit.LegacySupport.OldDll.MassTransit.ServiceBus.dll");
            var buffer = new byte[resourceStream.Length];
            int read = 0;
            while (read < resourceStream.Length)
            {
                read = resourceStream.Read(buffer, read, buffer.Length - read);
            }
            return Assembly.Load(buffer);
        }
    }
}