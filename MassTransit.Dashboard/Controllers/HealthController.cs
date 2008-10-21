namespace MassTransit.Dashboard.Controllers
{
    using System;
    using System.Collections.Generic;
    using Castle.MonoRail.Framework;
    using ServiceBus;
    using ServiceBus.Services.HealthMonitoring;
    using ServiceBus.Services.HealthMonitoring.Messages;

    [Layout("default")]
    public class HealthController :
        SmartDispatcherController,
        Consumes<HealthStatusResponse>.For<Guid>,
        Consumes<Pong>.For<Guid>
    {
        private readonly IServiceBus _bus;
        private ServiceBusRequest<HealthController> _request;
        private readonly Guid _correlationId = Guid.NewGuid();
        private HealthStatusResponse _response;
        private IHealthCache _cache;

        public HealthController(IServiceBus bus, IHealthCache cache)
        {
            _bus = bus;
            _cache = cache;
        }

        //public IAsyncResult BeginView()
        //{
        //    _request = _bus.Request()
        //        .From(this)
        //        .WithCallback(ControllerContext.Async.Callback, ControllerContext.Async.State);

        //    _request.Send(new HealthStatusRequest(_bus.Endpoint.Uri, this.CorrelationId));

        //    return _request;
        //}
        //public void EndView()
        //{
        //    //IAsyncResult r = ControllerContext.Async.Output; //TODO: Do I need this?
        //    PropertyBag.Add("statuses", _response.HealthInformation);
        //}

        public void View()
        {
            //IList<HealthInformation> infos = _cache.List();
            IList<HealthInformation> infos = new List<HealthInformation>();
            PropertyBag.Add("statuses", infos);
        }

        public void Ping(string uri)
        {
            _request = _bus.Request().From(this);

            _request.Send(new Ping(_correlationId));

            _request.AsyncWaitHandle.WaitOne(3000, true); //the consumes method will set the property bag
        }

        #region HealthStatusResponse

        public void Consume(HealthStatusResponse message)
        {
            _response = message;
            _request.Complete();
        }

        public Guid CorrelationId
        {
            get { return _correlationId; }
        }

        #endregion

        #region Pong
        public void Consume(Pong message)
        {
            //this.PropertyBag.Add("pong", message);
            this.RenderText("ponged");
            _request.Complete();
        }
        #endregion
    }
}