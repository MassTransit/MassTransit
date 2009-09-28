namespace MassTransit.Services.Routing
{
    using System;
    using System.Collections.Generic;
    using Internal;

    public class RoutingService :
        IBusService
    {
        
        UnsubscribeAction _unsubscribe;
        readonly IList<Func<IServiceBus, IEndpointFactory, UnsubscribeAction>> _routes;
        readonly IEndpointFactory _factory;

        public RoutingService(IList<Func<IServiceBus, IEndpointFactory, UnsubscribeAction>> routes, IEndpointFactory factory)
        {
            _routes = routes;
            _factory = factory;
        }

        public void Dispose()
        {
            
        }

        public void Start(IServiceBus bus)
        {
            _unsubscribe = () => true;

            foreach (Func<IServiceBus, IEndpointFactory, UnsubscribeAction> route in _routes)
            {
                _unsubscribe += route(bus, _factory);
            }
        }

        public void Stop()
        {
            _unsubscribe();
        }
    }
}