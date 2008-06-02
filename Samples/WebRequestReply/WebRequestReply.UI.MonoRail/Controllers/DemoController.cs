namespace WebRequestReply.UI.MonoRail.Controllers
{
    using System;
    using Castle.MonoRail.Framework;
    using Core;
    using MassTransit.ServiceBus;

    public class DemoController :
        SmartDispatcherController,
        Consumes<ResponseMessage>.For<Guid>
    {
        private IServiceBus _bus;
        private Guid _correlationId;

        public DemoController(IServiceBus bus)
        {
            _bus = bus;
            _correlationId = Guid.NewGuid();
        }

        public void Default()
        {
            
        }


        public void Sync(string requestText)
        {
            this.RenderText("MT: " + requestText);
        }

        //http://www.ayende.com/Blog/archive/2008/03/25/Async-Actions-in-Monorail.aspx
        public IAsyncResult BeginAsync(string requestText)
        {
            return AsyncRequest.From(this)
                .Via(_bus)
                .Send(new RequestMessage(this.CorrelationId, requestText));
        }

        public void EndAsync()
        {
            IAsyncResult r = ControllerContext.Async.Result;
            this.PropertyBag.Add("responseText", "dru");
            this.RenderView("Default");
        }

        #region Consumes<ResponseMessage>.All Members

        private ResponseMessage msg;
        public void Consume(ResponseMessage message)
        {
            msg = message;
        }

        #endregion

        #region CorrelatedBy<Guid> Members

        public Guid CorrelationId
        {
            get { return _correlationId; }
        }

        #endregion
    }
}