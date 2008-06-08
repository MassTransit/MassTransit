namespace MassTransit.ServiceBus.Tests
{
	using System;
	using System.IO;
	using System.Reflection;
	using log4net;
	using log4net.Config;
	using MassTransit.ServiceBus.Subscriptions;
	using NUnit.Framework;
	using Rhino.Mocks;

	public abstract class ServiceBusSetupFixture
	{
		protected static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		protected MockRepository _mocks = new MockRepository();
		protected IEndpoint _poisonEndpoint; // = @"msmq://localhost/test_servicebus_poison";
		protected string _poisonQueueName = @"msmq://localhost/test_servicebus_poison";

		protected ServiceBus _remoteServiceBus;

		protected IEndpoint _remoteServiceBusEndPoint; // = @"msmq://localhost/test_remoteservicebus";
		protected string _remoteServiceBusQueueName = @"msmq://localhost/test_remoteservicebus";
		protected ServiceBus _serviceBus;
		protected IEndpoint _serviceBusEndPoint; // = @"msmq://localhost/test_servicebus";
		protected string _serviceBusQueueName = @"msmq://localhost/test_servicebus";
		protected IEndpoint _testEndPoint; // = @"msmq://localhost/test_endpoint";
		protected string _testEndPointQueueName = @"msmq://localhost/test_endpoint";

		[SetUp]
		public virtual void Before_Each_Test_In_The_Fixture()
		{
			XmlConfigurator.Configure(new FileInfo(@".\log4net.config"));

			_serviceBusEndPoint = _mocks.CreateMock<IEndpoint>();
			_remoteServiceBusEndPoint = _mocks.CreateMock<IEndpoint>();
			_testEndPoint = _mocks.CreateMock<IEndpoint>();
			_poisonEndpoint = _mocks.CreateMock<IEndpoint>();

			SetupResult.For(_serviceBusEndPoint.Uri).Return(new Uri(_serviceBusQueueName));
			SetupResult.For(_remoteServiceBusEndPoint.Uri).Return(new Uri(_remoteServiceBusQueueName));
			SetupResult.For(_testEndPoint.Uri).Return(new Uri(_testEndPointQueueName));
			SetupResult.For(_testEndPoint.Uri).Return(new Uri(_testEndPointQueueName));

			_mocks.ReplayAll();	

			ISubscriptionCache _subscriptionCache = new LocalSubscriptionCache();

            ServiceBus bus = new ServiceBus(_serviceBusEndPoint, null, _subscriptionCache);

			bus.PoisonEndpoint = _poisonEndpoint;
			_serviceBus = bus;


			_remoteServiceBus = new ServiceBus(_remoteServiceBusEndPoint, null, _subscriptionCache);
		}

		[TearDown]
		public virtual void After_Each_Test_In_The_Fixture()
		{

		}
	}
}