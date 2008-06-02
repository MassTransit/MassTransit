namespace WebRequestReply.UI.MonoRail
{
    using System.Web;
    using Castle.Windsor;
    using Core;
    using MassTransit.ServiceBus;

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

        private static void HandleRequestMessage(IMessageContext<RequestMessage> ctx)
        {
            ResponseMessage response = new ResponseMessage(ctx.Message.CorrelationId, "Request: " + ctx.Message.Text);

            container.Resolve<IServiceBus>().Publish(response);
        }

        public IWindsorContainer Container
        {
            get { return container; }
        }
    }
}