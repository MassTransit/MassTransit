namespace MassTransit.Services.Routing
{
    using System;
    using System.Collections.Generic;
    using Internal;

    public class RoutingService :
        IBusService
    {
        
        UnsubscribeAction _unsubscribe;
        readonly IList<Func<IServiceBus, IEndpointResolver, UnsubscribeAction>> _routes;
        readonly IEndpointResolver _resolver;

        public RoutingService(IList<Func<IServiceBus, IEndpointResolver, UnsubscribeAction>> routes, IEndpointResolver resolver)
        {
            _routes = routes;
            _resolver = resolver;
        }

        public void Dispose()
        {
            
        }

        public void Start(IServiceBus bus)
        {
            _unsubscribe = () => true;

            foreach (Func<IServiceBus, IEndpointResolver, UnsubscribeAction> route in _routes)
            {
                _unsubscribe += route(bus, _resolver);
            }
        }

        public void Stop()
        {
            _unsubscribe();
        }
    }
}