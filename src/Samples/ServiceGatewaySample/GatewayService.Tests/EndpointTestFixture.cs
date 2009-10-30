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
namespace GatewayService.Tests
{
	using System;
	using Magnum.DateTimeExtensions;
	using MassTransit;
	using MassTransit.Configuration;
	using MassTransit.Saga;
	using MassTransit.Serialization;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public abstract class EndpointTestFixture<TTransport>
		where TTransport : IEndpoint
	{
		[SetUp]
		public void Setup()
		{
			ObjectBuilder = MockRepository.GenerateMock<IObjectBuilder>();

			XmlMessageSerializer serializer = new XmlMessageSerializer();
			ObjectBuilder.Stub(x => x.GetInstance<XmlMessageSerializer>()).Return(serializer);

			EndpointFactory = EndpointFactoryConfigurator.New(x =>
				{
					x.SetObjectBuilder(ObjectBuilder);
					x.RegisterTransport<TTransport>();
					x.SetDefaultSerializer<XmlMessageSerializer>();

					AdditionalEndpointFactoryConfiguration(x);
				});
			ObjectBuilder.Stub(x => x.GetInstance<IEndpointFactory>()).Return(EndpointFactory);

			ServiceBusConfigurator.Defaults(x =>
				{
					x.SetObjectBuilder(ObjectBuilder);
					x.SetReceiveTimeout(500.Milliseconds());
					x.SetConcurrentConsumerLimit(Environment.ProcessorCount*2);
				});

			EstablishContext();
		}

		[TearDown]
		public void Teardown()
		{
			TeardownContext();

			EndpointFactory.Dispose();
			EndpointFactory = null;
		}

		protected virtual void AdditionalEndpointFactoryConfiguration(IEndpointFactoryConfigurator x)
		{
		}

		protected IEndpointFactory EndpointFactory { get; set; }

		protected IObjectBuilder ObjectBuilder { get; private set; }

		protected virtual void EstablishContext()
		{
		}

		protected virtual void TeardownContext()
		{
		}

		public static InMemorySagaRepository<TSaga> SetupSagaRepository<TSaga>(IObjectBuilder builder)
			where TSaga : class, ISaga
		{
			var sagaRepository = new InMemorySagaRepository<TSaga>();

			builder.Stub(x => x.GetInstance<ISagaRepository<TSaga>>())
				.Return(sagaRepository);

			return sagaRepository;
		}
	}
}