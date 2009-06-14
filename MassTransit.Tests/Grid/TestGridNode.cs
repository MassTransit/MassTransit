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
	using MassTransit.Grid;
	using MassTransit.Grid.Configuration;
	using MassTransit.Grid.Messages;
	using MassTransit.Grid.Paxos;
	using MassTransit.Grid.Sagas;
	using MassTransit.Saga;
	using MassTransit.Services.Subscriptions.Configuration;
	using MassTransit.Transports;
	using Rhino.Mocks;
	using TextFixtures;

	public class TestGridNode :
		IDisposable
	{
		private volatile bool _disposed;

		public TestGridNode(string name, IEndpointFactory endpointFactory, string subscriptionServiceEndpointAddress, Action<GridConfigurator> configureGrid)
		{
			ObjectBuilder = MockRepository.GenerateMock<IObjectBuilder>();
			ObjectBuilder.Stub(x => x.GetInstance<IEndpointFactory>()).Return(endpointFactory);

			ControlBus = ControlBusConfigurator.New(x =>
				{
					x.SetObjectBuilder(ObjectBuilder);
					x.ReceiveFrom("loopback://localhost/mt_grid_" + name + "_control");

					x.PurgeBeforeStarting();
				});

			SetupGridNodeRepository();
			SetupGridServiceRepository();
			SetupGridServiceNodeRepository();
			SetupGridListenerRepository();
			SetupGridAcceptorRepository();

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

					x.ConfigureService(configureGrid);
				});
		}

		public ISagaRepository<GridNode> GridNodeRepository { get; private set; }
		public ISagaRepository<GridService> GridServiceRepository { get; private set; }
		public ISagaRepository<GridServiceNode> GridServiceNodeRepository { get; private set; }
		public ISagaRepository<Listener<AvailableGridServiceNode>> GridListenerRepository { get; private set; }
		public ISagaRepository<Acceptor<AvailableGridServiceNode>> GridAcceptorRepository { get; private set; }

		public IObjectBuilder ObjectBuilder { get; private set; }
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

			DataBus.Dispose();
			DataBus = null;

			ControlBus.Dispose();
			ControlBus = null;

			_disposed = true;
		}

		private void SetupGridNodeRepository()
		{
			GridNodeRepository = EndpointTestFixture<LoopbackEndpoint>.SetupSagaRepository<GridNode>(ObjectBuilder);
			EndpointTestFixture<LoopbackEndpoint>
				.SetupObservesSagaStateMachineSink<GridNode, NotifyNodeAvailable>(ControlBus, GridNodeRepository, ObjectBuilder);
			EndpointTestFixture<LoopbackEndpoint>
				.SetupObservesSagaStateMachineSink<GridNode, NotifyNodeDown>(ControlBus, GridNodeRepository, ObjectBuilder);
			EndpointTestFixture<LoopbackEndpoint>
				.SetupObservesSagaStateMachineSink<GridNode, NotifyNodeWorkload>(ControlBus, GridNodeRepository, ObjectBuilder);
		}

		private void SetupGridServiceRepository()
		{
			GridServiceRepository = EndpointTestFixture<LoopbackEndpoint>.SetupSagaRepository<GridService>(ObjectBuilder);
			EndpointTestFixture<LoopbackEndpoint>
				.SetupObservesSagaStateMachineSink<GridService, GridServiceAddedToNode>(ControlBus, GridServiceRepository, ObjectBuilder);
		}

		private void SetupGridListenerRepository()
		{
			GridListenerRepository = EndpointTestFixture<LoopbackEndpoint>.SetupSagaRepository<Listener<AvailableGridServiceNode>>(ObjectBuilder);
			EndpointTestFixture<LoopbackEndpoint>
				.SetupObservesSagaStateMachineSink<Listener<AvailableGridServiceNode>, Accepted<AvailableGridServiceNode>>(ControlBus, GridListenerRepository, ObjectBuilder);
		}

		private void SetupGridAcceptorRepository()
		{
			GridAcceptorRepository = EndpointTestFixture<LoopbackEndpoint>.SetupSagaRepository<Acceptor<AvailableGridServiceNode>>(ObjectBuilder);
			EndpointTestFixture<LoopbackEndpoint>
				.SetupObservesSagaStateMachineSink<Acceptor<AvailableGridServiceNode>, Prepare<AvailableGridServiceNode>>(ControlBus, GridAcceptorRepository, ObjectBuilder);
			EndpointTestFixture<LoopbackEndpoint>
				.SetupObservesSagaStateMachineSink<Acceptor<AvailableGridServiceNode>, Accept<AvailableGridServiceNode>>(ControlBus, GridAcceptorRepository, ObjectBuilder);
		}

		private void SetupGridServiceNodeRepository()
		{
			GridServiceNodeRepository = EndpointTestFixture<LoopbackEndpoint>.SetupSagaRepository<GridServiceNode>(ObjectBuilder);
			EndpointTestFixture<LoopbackEndpoint>
				.SetupObservesSagaStateMachineSink<GridServiceNode, AddGridServiceToNode>(ControlBus, GridServiceNodeRepository, ObjectBuilder);
			EndpointTestFixture<LoopbackEndpoint>
				.SetupObservesSagaStateMachineSink<GridServiceNode, RemoveGridServiceFromNode>(ControlBus, GridServiceNodeRepository, ObjectBuilder);
		}

		~TestGridNode()
		{
			Dispose(false);
		}
	}
}