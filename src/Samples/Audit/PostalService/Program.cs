namespace PostalService
{
	using System.IO;
	using Castle.Windsor;
	using Host;
	using log4net.Config;
	using Topshelf;
	using Topshelf.Configuration.Dsl;

    internal class Program
	{
		private static void Main(string[] args)
		{
			XmlConfigurator.ConfigureAndWatch(new FileInfo("log4net.xml"));

			var cfg = RunnerConfigurator.New(c =>
				{
					c.SetServiceName("PostalService");
					c.SetDisplayName("Sample Email Service");
					c.SetDescription("we goin' postal");

					c.RunAsLocalSystem();
					c.DependencyOnMsmq();

					c.BeforeStartingServices(a =>
						{
							var container = new WindsorContainer("postal-castle.xml");
							container.AddComponent<SendEmailConsumer>("sec");
							container.AddComponent<PostalService>();
						});

					c.ConfigureService<PostalService>(a =>
						{
							a.WhenStarted(o => o.Start());
							a.WhenStopped(o => o.Stop());
						});
				});
			Runner.Host(cfg, args);
		}
	}
}