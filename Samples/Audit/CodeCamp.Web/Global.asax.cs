namespace CodeCamp.Web
{
    using System;
    using System.Web;
    using Domain;
    using log4net.Config;
    using Magnum.Common;
    using Magnum.Common.Data;
    using Magnum.Infrastructure.Data;
    using MassTransit;
    using MassTransit.WindsorIntegration;
    using NHibernate;
    using NHibernate.Cfg;

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

			Configuration _cfg = new Configuration().Configure();

			_cfg.AddAssembly(typeof(NHibernateRepositoryFactory).Assembly);

			ISessionFactory _sessionFactory = _cfg.BuildSessionFactory();

			LocalContext.Current.Store(_sessionFactory);

			NHibernateUnitOfWork.SetSessionProvider(_sessionFactory.OpenSession);

			UnitOfWork.SetUnitOfWorkProvider(NHibernateUnitOfWork.Create);

			_container.Kernel.AddComponentInstance<IRepositoryFactory>(NHibernateRepositoryFactory.Configure(x => { }));

            DomainContext.Initialize(_container.Resolve<IServiceBus>("client"), _container.Resolve<IObjectBuilder>());
        }

        protected void Application_End(object sender, EventArgs e)
        {
        }
    }
}