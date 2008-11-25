namespace WebRequestReply.UI.MonoRail
{
    using System.Web;
    using Castle.Windsor;
    using Core;
    using MassTransit;

    public class DashboardApplication :
        HttpApplication, IContainerAccessor
    {
        private static WebAppContainer container;

        public void Application_OnStart()
        {
            container = new WebAppContainer();

            container.Resolve<IServiceBus>().Subscribe<RequestMessage>(HandleRequestMessage);
        }

        public void Application_OnEnd()
        {
            container.Dispose();
        }

        private static void HandleRequestMessage(RequestMessage message)
        {
            ResponseMessage response = new ResponseMessage(message.CorrelationId, "Request: " + message.Text);

            container.Resolve<IServiceBus>().Publish(response);
        }

        public IWindsorContainer Container
        {
            get { return container; }
        }
    }
}