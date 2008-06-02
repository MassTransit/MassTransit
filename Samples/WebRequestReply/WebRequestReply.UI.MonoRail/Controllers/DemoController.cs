namespace WebRequestReply.UI.MonoRail.Controllers
{
    using System;
    using Castle.MonoRail.Framework;
    using MassTransit.ServiceBus;

    public class DemoController :
        SmartDispatcherController
    {
        private IServiceBus _bus;


        public DemoController(IServiceBus bus)
        {
            _bus = bus;
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
            return null; //return IAsyncResult
        }

        public void EndAsync()
        {
            this.RenderText("MT: ??");
        }
    }
}