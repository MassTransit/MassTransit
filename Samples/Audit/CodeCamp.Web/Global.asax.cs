namespace CodeCamp.Web
{
    using System;
    using System.Web;
    using Domain;
    using MassTransit.ServiceBus;
    using MassTransit.WindsorIntegration;

    public class Global : HttpApplication
    {
        private DefaultMassTransitContainer _container;

        protected void Application_Start(object sender, EventArgs e)
        {
            _container = new DefaultMassTransitContainer(Server.MapPath("web-castle.config"));

            DomainContext.Initialize(_container.Resolve<IServiceBus>("client"));
        }

        protected void Application_End(object sender, EventArgs e)
        {
        }
    }
}