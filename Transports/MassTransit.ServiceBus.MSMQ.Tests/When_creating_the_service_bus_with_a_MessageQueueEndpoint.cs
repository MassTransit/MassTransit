namespace MassTransit.ServiceBus.MSMQ.Tests
{
	using System;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;
	using Rhino.Mocks;
	using Subscriptions;

	[TestFixture]
	public class When_creating_the_service_bus_with_a_MessageQueueEndpoint
	{

		[SetUp]
		public void SetUp()
		{
			_mocks = new MockRepository();
		}

		[TearDown]
		public void TearDown()
		{
			_mocks = null;
		}

		private MockRepository _mocks;

		[Test]
		public void The_endpoint_Uri_should_match_the_original_endpoint_Uri()
		{
			string endpointName = @"msmq://localhost/test_servicebus";

			MsmqEndpoint defaultEndpoint = endpointName;

			ServiceBus serviceBus = new ServiceBus(defaultEndpoint, _mocks.CreateMock<IObjectBuilder>());

			string machineEndpointName = endpointName.Replace("localhost", Environment.MachineName.ToLowerInvariant());

			Assert.That(serviceBus.Endpoint.Uri.AbsoluteUri, Is.EqualTo(machineEndpointName));
		}
	}
}