// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Tests.Services.Routing
{
	using System;
	using MassTransit.Internal;
	using MassTransit.Pipeline;
	using MassTransit.Pipeline.Configuration;
	using MassTransit.Pipeline.Inspectors;
	using MassTransit.Services.Routing.Configuration;
	using Messages;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class RoutingConfiguration_Specs
	{
		[SetUp]
		public void Setup_Context()
		{
			_builder = MockRepository.GenerateStub<IObjectBuilder>();
			_bus = MockRepository.GenerateStub<IServiceBus>();
			_pipeline = MessagePipelineConfigurator.CreateDefault(_builder, _bus);
			var endpointCache = MockRepository.GenerateStub<IEndpointCache>();
			_address = new Uri("msmq://localhost/dru");
			_builder.Stub(x => x.GetInstance<IEndpointCache>()).Return(endpointCache);
			endpointCache.Stub(x => x.GetEndpoint(_address)).Return(new NullEndpoint());

			_bus.Stub(x => x.OutboundPipeline).Return(_pipeline);
		}

		Uri _address;
		IServiceBus _bus;
		MessagePipeline _pipeline;
		IObjectBuilder _builder;

		[Test]
		public void ConfigurationTest()
		{
			var configurator = new RoutingConfigurator();

			configurator.Route<PingMessage>().To(_address);
			configurator.Route<PongMessage>().To(_address);


			IBusService busService = configurator.Create(_bus);

			PipelineViewer.Trace(_pipeline);

			busService.Start(_bus);

			PipelineViewer.Trace(_pipeline);

			busService.Stop();

			PipelineViewer.Trace(_pipeline);
		}
	}
}