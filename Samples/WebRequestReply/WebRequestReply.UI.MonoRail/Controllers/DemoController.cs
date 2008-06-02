namespace WebRequestReply.UI.MonoRail.Controllers
{
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


        public void Post(string requestText)
        {
            this.RenderText("DDS:" + requestText);
        }
    }
}