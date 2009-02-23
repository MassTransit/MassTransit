namespace CodeCamp.Service
{
	using Domain;
	using log4net.Config;
	using Magnum.Common;
	using Magnum.Common.Data;
	using Magnum.Infrastructure.Data;
	using MassTransit.Infrastructure.Saga;
	using MassTransit.Saga;
	using MassTransit.WindsorIntegration;
	using NHibernate;
	using NHibernate.Cfg;
	using Topshelf;
	using Topshelf.Configuration;

    internal class Program
	{
		private static void Main(string[] args)
		{
			XmlConfigurator.Configure();

		    var cfg = RunnerConfigurator.New(c =>
		                                         {
                                                     c.RunAsLocalSystem();
		                                             c.SetServiceName("audit");
		                                             c.SetDisplayName("audit");
		                                             c.SetDescription("audit");

                                                     c.BeforeStart(a=>
                                                                       {
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
                                                                           container.AddComponent<AuditService>();
                                                                           
                                                                       });

                                                     c.ConfigureService<AuditService>(a=>
                                                                                                   {
                                                                                                       a.WhenStarted(o=>o.Start());
                                                                                                       a.WhenStopped(o=>o.Stop());
                                                                                                   });
		                                         });
			Runner.Host(cfg, args);
		}
	}
}