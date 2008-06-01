namespace MassTransit.ServiceBus.HealthMonitoring
{
    using System;
    using Messages;

    public class Investigator : 
        Consumes<Suspect>.All,
        Consumes<Pong>.For<Guid>//, Produces<DownEndpoint>
    {
        private readonly IServiceBus _bus;
        private Guid _correlationId;

        public Investigator(IServiceBus bus)
        {
            _correlationId = Guid.NewGuid();
            _bus = bus;
        }


        //this starts things
        public void Consume(Suspect message)
        {
            _bus.Publish(new Ping(Guid.NewGuid()));
        }

        //produce<Ping>
        public void Consume(Pong message)
        {
            //if we get this we are ok. but its weird that the heartbeat is down

            //on time out
            //I have a confirmed dead endpoint
            //_bus.Publish<DownEndpoint>(new DownEndpoint());

        }

        public Guid CorrelationId
        {
            get { return _correlationId; }
        }

    }
}