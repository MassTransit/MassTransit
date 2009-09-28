namespace MassTransit.Dashboard
{
    using System.Web;
    using Castle.Windsor;
    using Microsoft.Practices.ServiceLocation;
    using WindsorIntegration;

    public class DashboardApplication :
        HttpApplication, IContainerAccessor
    {
        private static WebAppContainer container;

        public void Application_OnStart()
        {
            container = new WebAppContainer();
            var wob = new WindsorObjectBuilder(container.Kernel);
            ServiceLocator.SetLocatorProvider(() => wob);
        }

        public void Application_OnEnd()
        {
            container.Dispose();
        }

        public IWindsorContainer Container
        {
            get { return container; }
        }
    }
}