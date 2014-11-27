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
namespace MassTransit.Tests.Saga
{
	using System;
	using System.Linq;
	using Magnum.TestFramework;
	using MassTransit.Pipeline;
	using MassTransit.Saga;
	using NUnit.Framework;
	using TestFramework;
	using TextFixtures;

	[TestFixture]
	public class When_an_initiating_message_for_a_saga_arrives :
		LoopbackTestFixture
	{
		protected override void EstablishContext()
		{
			base.EstablishContext();

			_sagaId = Guid.NewGuid();

			_repository = SetupSagaRepository<SimpleSaga>();

			_remove = LocalBus.SubscribeSaga<SimpleSaga>(_repository);

		}

		private Guid _sagaId;
		private UnsubscribeAction _remove;
		private ISagaRepository<SimpleSaga> _repository;

		[Test]
		public void The_saga_should_be_created_when_an_initiating_message_is_received()
		{
			InitiateSimpleSaga message = new InitiateSimpleSaga(_sagaId);

			LocalBus.Endpoint.Send(message);

			_repository.ShouldContainSaga(_sagaId).ShouldNotBeNull();
		}

		[Test]
		public void The_saga_should_be_loaded_when_an_orchestrated_message_is_received()
		{
            LocalBus.Endpoint.Send(new InitiateSimpleSaga(_sagaId));

            LocalBus.Endpoint.Send(new CompleteSimpleSaga(_sagaId));

			var saga = _repository.ShouldContainSaga(_sagaId);

			saga.Completed.ShouldBeTrue();
		}

		[Test]
		public void The_saga_should_be_loaded_when_an_observed_message_is_received()
		{
			const string name = "Chris";

            LocalBus.Endpoint.Send(new InitiateSimpleSaga(_sagaId) { Name = name });

            LocalBus.Endpoint.Send(new ObservableSagaMessage { Name = name });

			var saga = _repository.ShouldContainSaga(_sagaId);

			saga.Observed.ShouldBeTrue();
		}
	}

	[TestFixture]
	public class When_an_existing_saga_receives_an_initiating_message :
		LoopbackTestFixture
	{
		protected override void EstablishContext()
		{
			base.EstablishContext();

			_sagaId = Guid.NewGuid();

			_repository = SetupSagaRepository<SimpleSaga>();

			LocalBus.SubscribeSaga(_repository);
		}

		private Guid _sagaId;
		private ISagaRepository<SimpleSaga> _repository;

		[Test]
		public void An_exception_should_be_thrown()
		{
			InitiateSimpleSaga message = new InitiateSimpleSaga(_sagaId);

            LocalBus.Endpoint.Send(message);

			try
			{
                LocalBus.Endpoint.Send(message);
			}
			catch (SagaException sex)
			{
				Assert.AreEqual(sex.MessageType, typeof(InitiateSimpleSaga));
			}
		}
	}
}