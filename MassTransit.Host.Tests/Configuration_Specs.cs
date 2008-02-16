namespace MassTransit.Host.Tests
{
	using System.Collections.Generic;
	using System.IO;
	using System.Reflection;
	using log4net;
	using log4net.Config;
	using NUnit.Framework;
	using ServiceBus;

	[TestFixture]
	public class When_an_endpoint_is_specified
	{
		private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private Dictionary<string, IEndpoint> _endpoints = new Dictionary<string, IEndpoint>();

		[Test]
		public void Create_a_service_bus_for_the_endpoint()
		{
			IConfigurator configurator = new HostConfigurator();

			Controller controller = new Controller();
			controller.Configure(configurator);
		}

		[Test]
		public void How_well_does_the_reader_work()
		{
			XmlConfigurator.Configure(new FileInfo(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "log4net.config")));

			IConfigurator configurator = new HostConfigurator();

			configurator.Configure();
		}


		//while (xmlReader.Read())
		//{
		//    if(xmlReader.NodeType == XmlNodeType.Element)
		//    {
		//        switch (xmlReader.LocalName)
		//        {
		//            case "assembly":
		//                xmlReader.Read();
		//                string assemblyName = xmlReader.Value;
		//                xmlReader.Read();
		//                if(xmlReader.LocalName == "endpoint-ref")
	}
}