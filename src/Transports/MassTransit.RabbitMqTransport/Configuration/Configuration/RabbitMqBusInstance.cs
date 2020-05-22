namespace MassTransit.RabbitMqTransport.Configuration
{
    using System;
    using Registration;


    public class RabbitMqBusInstance :
        IBusInstance
    {
        readonly IRabbitMqHostConfiguration _hostConfiguration;

        public RabbitMqBusInstance(IBusControl busControl, IRabbitMqHostConfiguration hostConfiguration)
        {
            _hostConfiguration = hostConfiguration;
            BusControl = busControl;
        }

        public Type InstanceType => typeof(IBus);
        public IBus Bus => BusControl;
        public IBusControl BusControl { get; }
    }
}
