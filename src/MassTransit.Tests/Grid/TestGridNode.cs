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
					x.ReceiveFrom(name + "_control");

					x.PurgeBeforeStarting();
				});

			SetupGridNodeRepository();
			SetupGridServiceRepository();
			SetupGridServiceNodeRepository();
			SetupGridMessageNodeRepository();
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
					x.ReceiveFrom(name);
					x.UseControlBus(ControlBus);
					x.SetConcurrentConsumerLimit(4);

					x.ConfigureService(configureGrid);
				});
		}

		public ISagaRepository<Node> GridNodeRepository { get; private set; }
		public ISagaRepository<ServiceType> GridServiceRepository { get; private set; }
		public ISagaRepository<ServiceMessage> GridMessageNodeRepository { get; private set; }
		public ISagaRepository<ServiceNode> GridServiceNodeRepository { get; private set; }
		public ISagaRepository<Learner<AvailableGridServiceNode>> GridListenerRepository { get; private set; }
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
			GridNodeRepository = EndpointTestFixture<LoopbackEndpoint>.SetupSagaRepository<Node>(ObjectBuilder);
		}

		private void SetupGridServiceRepository()
		{
			GridServiceRepository = EndpointTestFixture<LoopbackEndpoint>.SetupSagaRepository<ServiceType>(ObjectBuilder);
		}

		private void SetupGridMessageNodeRepository()
		{
			GridMessageNodeRepository = EndpointTestFixture<LoopbackEndpoint>.SetupSagaRepository<ServiceMessage>(ObjectBuilder);
		}

		private void SetupGridListenerRepository()
		{
			GridListenerRepository = EndpointTestFixture<LoopbackEndpoint>.SetupSagaRepository<Learner<AvailableGridServiceNode>>(ObjectBuilder);
		}

		private void SetupGridAcceptorRepository()
		{
			GridAcceptorRepository = EndpointTestFixture<LoopbackEndpoint>.SetupSagaRepository<Acceptor<AvailableGridServiceNode>>(ObjectBuilder);
		}

		private void SetupGridServiceNodeRepository()
		{
			GridServiceNodeRepository = EndpointTestFixture<LoopbackEndpoint>.SetupSagaRepository<ServiceNode>(ObjectBuilder);
		}

		~TestGridNode()
		{
			Dispose(false);
		}
	}
}