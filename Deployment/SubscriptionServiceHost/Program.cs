using log4net.Config;

[assembly : XmlConfigurator(ConfigFile = "log4net.xml", Watch = true)]

namespace SubscriptionServiceHost
{
	using Castle.Core;
	using log4net;
	using MassTransit.Host;
	using MassTransit.ServiceBus;
	using MassTransit.ServiceBus.Subscriptions;
	using MassTransit.SubscriptionStorage;

	internal class Program
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (Program));

		private static void Main(string[] args)
		{
			_log.Info("SubMgr Loading");

			HostedEnvironment env = new SubscriptionManagerEnvironment("pubsub.castle.xml");

			env.Container.AddComponentLifeStyle("followerrepository", typeof (FollowerRepository), LifestyleType.Singleton);

			env.Container.AddComponent<IHostedService, SubscriptionService>();

			env.Container.AddComponent<ISubscriptionRepository, NHibernateSubscriptionStorage>();

			Runner.Run(env, args);
		}
	}
}