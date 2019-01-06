// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the
// License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, either express or implied. See the License for the
// specific language governing permissions and limitations under the License.
namespace MassTransit.ApplicationInsights.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using MassTransit.TestFramework;
	using MassTransit.Testing.Observers;
	using Microsoft.ApplicationInsights;
	using Microsoft.ApplicationInsights.Channel;
	using Microsoft.ApplicationInsights.Extensibility;
	using Microsoft.ApplicationInsights.Extensibility.Implementation;
	using NUnit.Framework;

	[TestFixture]
	public class TelemetryDependencies_Specs
       : InMemoryTestFixture
	{
		private TelemetryClient _telemetryClient;
		private List<ITelemetry> _sendItems;

		public TelemetryDependencies_Specs()
		{
			var configuration = new TelemetryConfiguration();
			_sendItems = new List<ITelemetry>();
			configuration.TelemetryChannel = new StubTelemetryChannel { OnSend = item => _sendItems.Add(item) };
			configuration.InstrumentationKey = Guid.NewGuid().ToString();
			configuration.TelemetryInitializers.Add(new OperationCorrelationTelemetryInitializer());
			_telemetryClient = new TelemetryClient(configuration);
		}

		[OneTimeSetUp]
		public void Telemetry_SpecsOneTimeSetup()
		{
			_observer = GetConsumeObserver<FakeMessage>();
			Bus.ConnectConsumeMessageObserver(_observer);
		}

		[SetUp]
		public void Telemetry_SpecsSetup()
		{
			_sendItems.Clear();
		}

		protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator cfg)
		{
			base.ConfigureInMemoryBus(cfg);
			cfg.UseApplicationInsightsOnConsume(_telemetryClient);
			cfg.UseApplicationInsightsOnPublish(_telemetryClient);
			cfg.UseApplicationInsightsOnSend(_telemetryClient);
		}

        Task<ConsumeContext<FakeMessage>> _handled;
		private TestConsumeMessageObserver<FakeMessage> _observer;

		protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
		{
			_handled= Handled<FakeMessage>(configurator);
		}

		[Test]
        [Category("Flakey")]
		public async Task Should_track_send_message()
		{
			await InputQueueSendEndpoint.Send(new FakeMessage());
            var context = await _handled;

			var post = await _observer.PostConsumed;

			Assert.AreEqual(2, _sendItems.Count);
			//check root id
			Assert.AreEqual(_sendItems[0].Context.Operation.Id, _sendItems[1].Context.Operation.Id);
			//check parent id
			Assert.AreEqual((_sendItems[0] as OperationTelemetry).Id, _sendItems[1].Context.Operation.ParentId);
		}

		[Test]
		public async Task Should_track_publish_message()
		{
			await Bus.Publish(new FakeMessage());

			var context = await _handled;

			var post = await _observer.PostConsumed;

			Assert.AreEqual(2, _sendItems.Count);
			//check root id
			Assert.AreEqual(_sendItems[0].Context.Operation.Id, _sendItems[1].Context.Operation.Id);
			//check parent id
			Assert.AreEqual((_sendItems[0] as OperationTelemetry).Id, _sendItems[1].Context.Operation.ParentId);
		}
	}

	public class FakeMessage
	{
		public Guid CorrelationId { get; private set; } = new Guid();
	}
}
