namespace MassTransit.Registration
{
    using System;


    public class DefaultBusInstance :
        IBusInstance
    {
        public DefaultBusInstance(IBusControl busControl)
        {
            BusControl = busControl;
        }

        public Type InstanceType => typeof(IBus);
        public IBus Bus => BusControl;
        public IBusControl BusControl { get; }
        public IBusConnector BusConnector => DefaultBusConnector.Instance.Value;
    }
}
