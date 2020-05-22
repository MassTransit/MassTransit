namespace MassTransit.RabbitMqTransport.Configuration
{
    using System;
    using Registration;


    public class RabbitMqBusInstance :
        IBusInstance
    {
        public RabbitMqBusInstance(IBusControl busControl, IRabbitMqHostConfiguration hostConfiguration)
        {
            BusControl = busControl;
            BusConnector = new RabbitMqBusConnector(hostConfiguration);
        }

        public Type InstanceType => typeof(IBus);
        public IBus Bus => BusControl;
        public IBusControl BusControl { get; }

        public IBusConnector BusConnector { get; }
    }
}
