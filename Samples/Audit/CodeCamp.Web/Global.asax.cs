namespace CodeCamp.Web
{
    using System;
    using System.Web;
    using Domain;
    using log4net.Config;
    using Magnum.Common.Repository;
    using Magnum.Infrastructure.Repository;
    using MassTransit;
    using MassTransit.WindsorIntegration;

    public class Global : HttpApplication
    {
        private DefaultMassTransitContainer _container;

        public DefaultMassTransitContainer Container
        {
            get { return _container; }
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            XmlConfigurator.Configure();

            _container = new DefaultMassTransitContainer(Server.MapPath("/web-castle.config"));

            _container.Kernel.AddComponentInstance<IRepositoryFactory>(NHibernateRepositoryFactory.Build());

            DomainContext.Initialize(_container.Resolve<IServiceBus>("client"), _container.Resolve<IObjectBuilder>());
        }

        protected void Application_End(object sender, EventArgs e)
        {
        }
    }
}