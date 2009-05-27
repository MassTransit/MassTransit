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
namespace MassTransit.Tests.TextFixtures
{
	using System;
	using System.Collections;
	using System.Linq.Expressions;
	using Configuration;
	using Magnum.DateTimeExtensions;
	using Magnum.StateMachine;
	using MassTransit.Pipeline;
	using MassTransit.Saga;
	using MassTransit.Saga.Pipeline;
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

			//TODO: Is this how it should be set up?
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
				x.SetReceiveTimeout(50.Milliseconds());
				x.SetConcurrentConsumerLimit(Environment.ProcessorCount * 2);
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

		protected ISagaRepository<TSaga> SetupSagaRepository<TSaga>()
				where TSaga : class, ISaga
		{
			var sagaRepository = new InMemorySagaRepository<TSaga>();

			ObjectBuilder.Stub(x => x.GetInstance<ISagaRepository<TSaga>>())
				.Return(sagaRepository);

			return sagaRepository;
		}

		protected void SetupInitiateSagaSink<TSaga, TMessage>(IServiceBus bus, ISagaRepository<TSaga> repository)
			where TSaga : class, ISaga, InitiatedBy<TMessage>
			where TMessage : class, CorrelatedBy<Guid>
		{
			SetupSagaSinkStub(x => new CorrelatedSagaMessageSink<TSaga, TMessage>(x["context"] as ISubscriberContext, bus, repository, (ISagaPolicy<TSaga, TMessage>)x["policy"]));
		}

		protected void SetupOrchestrateSagaSink<TSaga, TMessage>(IServiceBus bus, ISagaRepository<TSaga> repository)
			where TSaga : class, ISaga, Orchestrates<TMessage>
			where TMessage : class, CorrelatedBy<Guid>
		{
			SetupSagaSinkStub(x => new CorrelatedSagaMessageSink<TSaga, TMessage>(x["context"] as ISubscriberContext, bus, repository, (ISagaPolicy<TSaga, TMessage>)x["policy"]));
		}

		protected void SetupObservesSagaSink<TSaga, TMessage>(IServiceBus bus, ISagaRepository<TSaga> repository)
			where TSaga : class, ISaga, Observes<TMessage,TSaga>
			where TMessage : class
		{
			SetupSagaSinkStub(x => new PropertySagaMessageSink<TSaga, TMessage>((ISubscriberContext)x["context"], bus, repository, (ISagaPolicy<TSaga, TMessage>)x["policy"], (Expression<Func<TSaga, TMessage, bool>>)x["selector"]));
		}

		protected void SetupInitiateSagaStateMachineSink<TSaga, TMessage>(IServiceBus bus, ISagaRepository<TSaga> repository)
			where TSaga : SagaStateMachine<TSaga>, ISaga 
			where TMessage : class, CorrelatedBy<Guid>
		{
			SetupSagaSinkStub(x => new CorrelatedSagaStateMachineMessageSink<TSaga, TMessage>(x["context"] as ISubscriberContext, bus, repository, (ISagaPolicy<TSaga, TMessage>)x["policy"], x["dataEvent"] as DataEvent<TSaga, TMessage>));
		}

		protected void SetupOrchestrateSagaStateMachineSink<TSaga, TMessage>(IServiceBus bus, ISagaRepository<TSaga> repository)
			where TSaga : SagaStateMachine<TSaga>, ISaga 
			where TMessage : class, CorrelatedBy<Guid>
		{
			SetupSagaSinkStub(x => new CorrelatedSagaStateMachineMessageSink<TSaga, TMessage>(x["context"] as ISubscriberContext, bus, repository, (ISagaPolicy<TSaga, TMessage>)x["policy"], x["dataEvent"] as DataEvent<TSaga, TMessage>));
		}

		protected void SetupObservesSagaStateMachineSink<TSaga, TMessage>(IServiceBus bus, ISagaRepository<TSaga> repository)
			where TSaga : SagaStateMachine<TSaga>, ISaga 
			where TMessage : class
		{
			SetupSagaSinkStub(x => new PropertySagaStateMachineMessageSink<TSaga, TMessage>(x["context"] as ISubscriberContext, bus, repository, (ISagaPolicy<TSaga,TMessage>)x["policy"], (Expression<Func<TSaga, TMessage, bool>>)x["selector"], x["dataEvent"] as DataEvent<TSaga, TMessage>));
		}

		private void SetupSagaSinkStub<TSink>(Func<Hashtable, TSink> createSink)
			where TSink : class
		{
			ObjectBuilder.Stub(x => x.GetInstance<TSink>(new Hashtable()))
				.IgnoreArguments()
				.Return(null)
				.WhenCalled(invocation =>
				            invocation.ReturnValue = createSink(((Hashtable) invocation.Arguments[0])));
		}
	}
}
