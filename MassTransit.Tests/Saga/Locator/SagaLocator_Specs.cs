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
namespace MassTransit.Tests.Saga.Locator
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using Magnum;
	using MassTransit.Saga;
	using NUnit.Framework;
	using Tests.Messages;

	[TestFixture]
	public class When_locating_a_saga_using_a_message
	{
		[SetUp]
		public void Setup()
		{
			_sagaId = CombGuid.Generate();

			_repository = new InMemorySagaRepository<TestSaga>();

			_repository.Create<PingMessage>(_sagaId, (s, m) => s.Name = "Joe").ToArray();
			_repository.Create<PingMessage>(CombGuid.Generate(), (s, m) => s.Name = "Chris").ToArray();
		}

		[TearDown]
		public void Teardown()
		{
			_repository.Dispose();
			_repository = null;
		}

		private Guid _sagaId;
		private InMemorySagaRepository<TestSaga> _repository;

		[Test]
		public void A_correlated_message_should_find_the_correct_saga()
		{
			var ping = new PingMessage(_sagaId);

			bool found = false;// locator.TryGetSagaForMessage(ping, out saga);

			Assert.IsTrue(found);
		}

		[Test]
		public void A_plain_message_should_find_the_correct_saga_using_a_property()
		{
			NameMessage name = new NameMessage {Name = "Joe"};

			bool found = false;// locator.TryGetSagaForMessage(name, out saga);

			Assert.IsTrue(found);
		}

		[Test]
		public void A_plain_message_with_an_unknown_relationship_should_not_find_it()
		{
			NameMessage name = new NameMessage {Name = "Tom"};

			//ISagaLocator<TestSaga, NameMessage> locator =
			//	new PropertySagaLocator<TestSaga, NameMessage>(_repository, new ExistingSagaPolicy<TestSaga, NameMessage>(),
			//		(s, m) => s.Name == m.Name);

			bool found = false;// locator.TryGetSagaForMessage(name, out saga);

		}

		[Test]
		public void A_nice_interface_should_be_available_for_defining_saga_locators()
		{
			IServiceBus bus = null;

			bus.Bind<PingMessage>().To<TestSaga>().ByCorrelationId();


			bus.Bind<NameMessage>().To<TestSaga>().By((saga, message) => saga.Name == message.Name);
		}

	}

	public static class ExtensionForSagaBinding
	{
		public static SagaConfigurationBinder<TMessage> Bind<TMessage>(this IServiceBus bus)
		{
			return new SagaConfigurationBinder<TMessage>();
		}

		public static IServiceBus ByCorrelationId<TSaga,TMessage>(this SagaConfigurationBinder<TSaga,TMessage> binder) 
			where TMessage : CorrelatedBy<Guid> 
			where TSaga : class, ISaga
		{
			// this should register something in the container to handle this message on demand

			throw new NotImplementedException();
		}

		public static IServiceBus By<TSaga,TMessage>(this SagaConfigurationBinder<TSaga,TMessage> binder, Expression<Func<TSaga,TMessage,bool>> expression)
		{

			throw new NotImplementedException();
			
		}
	}

	public class SagaConfigurationBinder<TMessage>
	{
		public SagaConfigurationBinder<TSaga,TMessage> To<TSaga>()
		{
			return new SagaConfigurationBinder<TSaga, TMessage>();
		}
	}

	public class SagaConfigurationBinder<TSaga,TMessage>
	{
	}


	public class Bind<TMessage>
	{
		public class To<TSaga>
		{
			public static void ByCorrelationId()
			{
			}
		}
	}


	public class NameMessage
	{
		public string Name { get; set; }
	}
}
