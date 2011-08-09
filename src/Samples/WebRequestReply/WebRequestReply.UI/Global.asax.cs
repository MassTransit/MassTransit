namespace WebRequestReply.UI
{
	using System;
	using System.IO;
	using System.Web;
	using Castle.Facilities.TypedFactory;
	using Castle.MicroKernel.Registration;
	using Castle.Windsor;
	using Core;
	using MassTransit;
	using log4net;
	using log4net.Config;

	public class Global : HttpApplication, IContainerAccessor
	{
		public const string ServiceUri = "rabbitmq://localhost/WebRequestReplyService";
		public const string UIUri = "rabbitmq://localhost/WebRequestReplyUI";
		static IServiceBus _coreBus;
		static IServiceBus _uiBus;

		public IWindsorContainer Container { get; protected set; }

		protected void Application_Start(object sender, EventArgs e)
		{
			var fi = new FileInfo(Server.MapPath("~/WebRequestReply.UI.log4net.xml"));
			XmlConfigurator.Configure(fi);

			BasicConfigurator.Configure();

			CreateUIContainer();
			CreateServiceContainer();
		}

		void CreateUIContainer()
		{
			Container = new WindsorContainer();
			Container.AddFacility<TypedFactoryFacility>();
			Container.Register(Component.For<IServiceBus>().UsingFactoryMethod((k, c) =>
				{
					Bus.Initialize(sbc =>
						{
							sbc.UseRabbitMqRouting();
							sbc.ReceiveFrom(UIUri);
							sbc.Subscribe(s => s.LoadFrom(Container));
						});

					return Bus.Instance;
				}).LifeStyle.Singleton);

			_uiBus = Container.Resolve<IServiceBus>();
		}

		static void CreateServiceContainer()
		{
			var coreContainer = new WindsorContainer();
			coreContainer.AddFacility<TypedFactoryFacility>();
			coreContainer.Register(Component.For<Service>(),
			                       Component.For<IServiceBus>().UsingFactoryMethod((k, c) =>
			                       	{
			                       		return ServiceBusFactory.New(sbc =>
			                       			{
			                       				sbc.UseRabbitMqRouting();
			                       				sbc.ReceiveFrom(ServiceUri);
			                       				sbc.Subscribe(s => s.LoadFrom(coreContainer));
			                       			});
			                       	}).LifeStyle.Singleton);

			_coreBus = coreContainer.Resolve<IServiceBus>();
		}

		protected void Application_End(object sender, EventArgs e)
		{
			Bus.Shutdown();

			if (_coreBus != null)
			{
				_coreBus.Dispose();
				_coreBus = null;
			}

			Container.Dispose();
			Container = null;

			LogManager.Shutdown();
		}

		protected void Session_Start(object sender, EventArgs e)
		{
		}

		protected void Application_BeginRequest(object sender, EventArgs e)
		{
		}

		protected void Application_AuthenticateRequest(object sender, EventArgs e)
		{
		}

		protected void Application_Error(object sender, EventArgs e)
		{
		}

		protected void Session_End(object sender, EventArgs e)
		{
		}
	}
}