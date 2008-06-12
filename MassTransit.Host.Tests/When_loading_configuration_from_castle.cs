namespace MassTransit.Host.Tests
{
	using System.IO;
	using System.Reflection;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;
	using ServiceBus;

	[TestFixture]
	public class When_loading_configuration_from_castle
	{
		[Test]
		public void The_services_should_be_available()
		{
            //string configFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "hostConfig.xml");
            //CastleConfigurator configurator = new CastleConfigurator();
            //configurator.ConfigFile = configFile;
            //configurator.Configure();

            //Assert.That(configurator.Services, Is.Not.Null);

            //int count = 0;
            //foreach (IHostedService service in configurator.Services)
            //{
            //    count++;
            //}

            //Assert.That(count, Is.EqualTo(1));
		}
	}
}