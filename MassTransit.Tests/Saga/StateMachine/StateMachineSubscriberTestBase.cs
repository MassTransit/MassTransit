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
namespace MassTransit.Tests.Saga.StateMachine
{
	using System;
	using System.Linq;
	using Locator;
	using MassTransit.Saga.Configuration;
	using NUnit.Framework;

	public class StateMachineSubscriberTestBase
	{
		protected EventInspectorResult<TestSaga>[] Results;

		[SetUp]
		public void Setup()
		{
			EstablishContext();
		}

		protected virtual void EstablishContext()
		{
			var saga = new TestSaga(Guid.NewGuid());

			var inspector = new SagaStateMachineEventInspector<TestSaga>();
			saga.Inspect(inspector);

			Results = inspector.GetResults().ToArray();
		}
	}
}