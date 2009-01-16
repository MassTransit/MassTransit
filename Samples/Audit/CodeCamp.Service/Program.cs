namespace CodeCamp.Service
{
	using Domain;
	using log4net.Config;
	using Magnum.Common;
	using Magnum.Common.Data;
	using Magnum.Infrastructure.Data;
	using MassTransit.Host;
	using MassTransit.Infrastructure.Saga;
	using MassTransit.Saga;
	using MassTransit.WindsorIntegration;
	using Microsoft.Practices.ServiceLocation;
	using NHibernate;
	using NHibernate.Cfg;

	internal class Program
	{
		private static void Main(string[] args)
		{
			XmlConfigurator.Configure();

			var container = new DefaultMassTransitContainer("audit-castle.config");

			Configuration _cfg = new Configuration().Configure();

			_cfg.AddAssembly(typeof(NHibernateRepositoryFactory).Assembly);

			ISessionFactory _sessionFactory = _cfg.BuildSessionFactory();

			LocalContext.Current.Store(_sessionFactory);

			NHibernateUnitOfWork.SetSessionProvider(_sessionFactory.OpenSession);

			UnitOfWork.SetUnitOfWorkProvider(NHibernateUnitOfWork.Create);

			container.AddComponent<ISagaRepository<RegisterUserSaga>, NHibernateSagaRepository<RegisterUserSaga>>();

			container.AddComponent<Responder>();
			container.AddComponent<RegisterUserSaga>();

			var credentials = Credentials.LocalSystem;
			var settings = WinServiceSettings.Custom(
				"Audit",
				"Audit",
				"Audit");
			var lifecycle = new AuditServiceLifeCycle(ServiceLocator.Current);

			Runner.Run(credentials, settings, lifecycle, args);
		}
	}
}