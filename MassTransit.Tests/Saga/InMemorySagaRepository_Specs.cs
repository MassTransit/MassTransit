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
	using Magnum;
	using MassTransit.Saga;
	using NUnit.Framework;

	[TestFixture]
	public class InMemorySagaRepository_Specs
	{
		[Test]
		public void Initiating_a_new_saga_while_another_is_in_progress_should_fail()
		{
			var repo = new InMemorySagaRepository<RegisterUserSaga>();

			InitiateSaga(repo, x => { });
			InitiateSaga(repo, x => { });

			int i = 0;
			foreach (RegisterUserSaga saga in repo)
			{
				i++;

				InitiateSaga(repo, x => { });
				InitiateSaga(repo, x => { });
			}

			i.ShouldEqual(2);
		}

		private static void InitiateSaga<T>(ISagaRepository<T> repository, Action<T> action)
			where T : class
		{
			var initiateNewSaga = repository.InitiateNewSaga(CombGuid.Generate());
			if (!initiateNewSaga.MoveNext())
				throw new InvalidOperationException("The saga could not be initiated");

			action(initiateNewSaga.Current);

			while (initiateNewSaga.MoveNext())
			{
			}
		}
	}
}