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
			var enumerator = _repository.InitiateNewSaga(_sagaId);
			enumerator.MoveNext();
			enumerator.Current.Name = "Joe";

			enumerator = _repository.InitiateNewSaga(CombGuid.Generate());
			enumerator.MoveNext();
			enumerator.Current.Name = "Chris";
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
			ISagaLocator<TestSaga, PingMessage> locator =
				new CorrelatedSagaLocator<TestSaga, PingMessage>(_repository);

			var ping = new PingMessage(_sagaId);

			TestSaga saga = locator.GetSagaForMessage(ping);

			Assert.AreEqual(_sagaId, saga.CorrelationId);
		}

		[Test]
		public void A_plain_message_should_find_the_correct_saga_using_a_property()
		{
			NameMessage name = new NameMessage {Name = "Joe"};

			ISagaLocator<TestSaga, NameMessage> locator =
				new PropertySagaLocator<TestSaga, NameMessage>(_repository,
					(s, m) => s.Name == m.Name);

			TestSaga saga = locator.GetSagaForMessage(name);

			Assert.AreEqual(_sagaId, saga.CorrelationId);
		}

		[Test]
		public void A_plain_message_with_an_unknown_relationship_should_not_find_it()
		{
			NameMessage name = new NameMessage {Name = "Tom"};

			ISagaLocator<TestSaga, NameMessage> locator =
				new PropertySagaLocator<TestSaga, NameMessage>(_repository,
					(s, m) => s.Name == m.Name);

			TestSaga saga = locator.GetSagaForMessage(name);

			Assert.IsNull(saga);
		}
	}

	public class NameMessage
	{
		public string Name { get; set; }
	}
}