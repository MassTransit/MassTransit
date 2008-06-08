namespace HeavyLoad.Correlated
{
    using System;
    using MassTransit.ServiceBus;

    internal class CorrelatedController : 
        Consumes<SimpleResponseMessage>.For<Guid>
    {
        private readonly IServiceBus _bus;
        private readonly Guid _id;
        private ServiceBusRequest<CorrelatedController> _request;
        private TimeSpan _timeout = TimeSpan.FromSeconds(60);


        public CorrelatedController(IServiceBus bus)
        {
            _bus = bus;

            _id = Guid.NewGuid();
        }

        public void Consume(SimpleResponseMessage message)
        {
            _request.Complete();
        }

        public Guid CorrelationId
        {
            get { return _id; }
        }

        public event Action<CorrelatedController> OnSuccess = delegate { };
        public event Action<CorrelatedController> OnTimeout = delegate { };

        public void SimulateRequestResponse()
        {
            _request = _bus.Request().From(this);
			
            _request.Send(new SimpleRequestMessage(_id), RequestMode.Synchronous, _timeout);

            if (_request.IsCompleted)
                OnSuccess(this);
            else
                OnTimeout(this);
        }
    }
}