// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.Tests.Grid
{
	using System;
	using Configuration;
	using MassTransit.Saga;
	using MassTransit.Services.Subscriptions.Configuration;
	using MassTransit.Transports;
	using Rhino.Mocks;
	using TextFixtures;

	public class TestGridNode :
		IDisposable
	{
		private volatile bool _disposed;

		public TestGridNode(string name, IEndpointFactory endpointFactory, string subscriptionServiceEndpointAddress)
		{
			ObjectBuilder = MockRepository.GenerateMock<IObjectBuilder>();
			ObjectBuilder.Stub(x => x.GetInstance<IEndpointFactory>()).Return(endpointFactory);

			ControlBus = ControlBusConfigurator.New(x =>
				{
					x.SetObjectBuilder(ObjectBuilder);
					x.ReceiveFrom("loopback://localhost/mt_grid_" + name + "_control");

					x.PurgeBeforeStarting();
				});

			DataBus = ServiceBusConfigurator.New(x =>
				{
					x.SetObjectBuilder(ObjectBuilder);
					x.ConfigureService<SubscriptionClientConfigurator>(y =>
						{
							// setup endpoint
							y.SetSubscriptionServiceEndpoint(subscriptionServiceEndpointAddress);
						});
					x.ReceiveFrom("loopback://localhost/mt_grid_" + name);
					x.UseControlBus(ControlBus);
				});

			NodeStateRepository = EndpointTestFixture<LoopbackEndpoint>.SetupSagaRepository<NodeState>(ObjectBuilder);
			EndpointTestFixture<LoopbackEndpoint>
				.SetupObservesSagaStateMachineSink<NodeState, NotifyNodeAvailable>(ControlBus, NodeStateRepository, ObjectBuilder);
			EndpointTestFixture<LoopbackEndpoint>
				.SetupObservesSagaStateMachineSink<NodeState, NotifyNodeDown>(ControlBus, NodeStateRepository, ObjectBuilder);
			EndpointTestFixture<LoopbackEndpoint>
				.SetupObservesSagaStateMachineSink<NodeState, NotifyNodeWorkload>(ControlBus, NodeStateRepository, ObjectBuilder);

			ServiceGrid = new ServiceGrid(ObjectBuilder.GetInstance<IEndpointFactory>(), ObjectBuilder.GetInstance<ISagaRepository<NodeState>>());
			ServiceGrid.Start(DataBus);
		}

		public ISagaRepository<NodeState> NodeStateRepository { get; private set; }

		public IObjectBuilder ObjectBuilder { get; private set; }

		public ServiceGrid ServiceGrid { get; private set; }
		public IControlBus ControlBus { get; private set; }
		public IServiceBus DataBus { get; private set; }

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public virtual void Dispose(bool disposing)
		{
			if (!disposing || _disposed) return;

			ServiceGrid.Stop();
			ServiceGrid = null;

			DataBus.Dispose();
			DataBus = null;

			ControlBus.Dispose();
			ControlBus = null;

			_disposed = true;
		}

		~TestGridNode()
		{
			Dispose(false);
		}
	}
}