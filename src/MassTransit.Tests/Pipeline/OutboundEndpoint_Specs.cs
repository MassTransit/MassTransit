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
namespace MassTransit.Tests.Pipeline
{
	using System;
	using Context;
	using MassTransit.Pipeline;
	using MassTransit.Pipeline.Configuration;
	using MassTransit.Pipeline.Inspectors;
	using Messages;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class When_pushing_a_message_through_an_outbound_pipeline
	{
		[SetUp]
		public void Setup()
		{
			_pipeline = new OutboundPipelineConfigurator(MockRepository.GenerateMock<IServiceBus>()).Pipeline;
		}

		IOutboundMessagePipeline _pipeline;

		[Test]
		public void The_endpoint_consumer_should_be_returned()
		{
			var endpoint = MockRepository.GenerateMock<IEndpoint>();
			endpoint.Stub(x => x.Address.Uri).Return(new Uri("msmq://localhost/queue_name"));

			_pipeline.ConnectEndpoint<PingMessage>(endpoint);

			PipelineViewer.Trace(_pipeline);

			var message = new PingMessage();

			var context = MockRepository.GenerateMock<ISendContext<PingMessage>>();
			endpoint.Expect(x => x.Send(context)).IgnoreArguments();

			_pipeline.Dispatch(message);

			endpoint.VerifyAllExpectations();
		}
	}
}