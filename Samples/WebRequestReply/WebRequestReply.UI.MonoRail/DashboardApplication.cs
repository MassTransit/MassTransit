namespace WebRequestReply.UI.MonoRail
{
    using System.Web;
    using Castle.Windsor;

    public class DashboardApplication :
        HttpApplication, IContainerAccessor
    {
        private static WebAppContainer container;

        public void Application_OnStart()
        {
            container = new WebAppContainer();
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