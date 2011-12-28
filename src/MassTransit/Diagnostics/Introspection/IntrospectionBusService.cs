namespace MassTransit.Diagnostics.Introspection
{
    public class IntrospectionBusService :
        IBusService,
        Consumes<GetBusStatus>.Context
    {
        IServiceBus _controlBus;
        UnsubscribeAction _unsubscribe;
        IServiceBus _bus;

        public void Dispose()
        {
            //no-op
        }

        public void Start(IServiceBus bus)
        {
            _bus = bus;
            _controlBus = bus.ControlBus;
            _unsubscribe = _controlBus.SubscribeInstance(this);
        }

        public void Stop()
        {
            _unsubscribe();
        }

        public void Consume(IConsumeContext<GetBusStatus> msgCxt)
        {
            var probe = _bus.Probe();
            var responseAddress = msgCxt.ResponseAddress;
            var ep = _bus.GetEndpoint(responseAddress);

            ep.Send(new BusStatus()
                {
                    Probe = probe
                });
        }
    }
}