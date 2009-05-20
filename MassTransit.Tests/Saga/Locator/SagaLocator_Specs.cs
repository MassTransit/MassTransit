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
	public class When_using_the_state_machine_with_noncorrelated_messages
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
		public void A_nice_interface_should_be_available_for_defining_saga_locators()
		{
		}
	}

	public class NameMessage
	{
		public string Name { get; set; }
	}
}
