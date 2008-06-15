namespace MassTransit.Dashboard.Controllers
{
    using System;
    using Castle.MonoRail.Framework;
    using MassTransit.ServiceBus.HealthMonitoring.Messages;
    using ServiceBus;

    [Layout("default")]
    public class HealthController :
        SmartDispatcherController,
        Consumes<HealthStatusResponse>.For<Guid>
    {
        private readonly IServiceBus _bus;
        private ServiceBusRequest<HealthController> _request;
        private readonly Guid _correlationId = Guid.NewGuid();
        private HealthStatusResponse _response;

        public HealthController(IServiceBus bus)
        {
            _bus = bus;
        }

        public IAsyncResult BeginView()
        {
            _request = _bus.Request()
                .From(this)
                .WithCallback(ControllerContext.Async.Callback, ControllerContext.Async.State);

            _request.Send(new HealthStatusRequest(_bus.Endpoint.Uri, this.CorrelationId));

            return _request;
        }
        public void EndView()
        {
            //IAsyncResult r = ControllerContext.Async.Result; //TODO: Do I need this?
            PropertyBag.Add("statuses", _response.HealthInformation);
        }

        #region Implementation of All

        public void Consume(HealthStatusResponse message)
        {
            _response = message;
            _request.Complete();
        }

        #endregion

        #region Implementation of CorrelatedBy<Guid>

        public Guid CorrelationId
        {
            get { return _correlationId; }
        }

        #endregion
    }
}