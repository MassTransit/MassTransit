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
namespace MassTransit.Saga.Configuration
{
	using System.Collections.Generic;
	using Magnum.StateMachine;

	public class SagaPolicyFactory :
		ISagaPolicyFactory
	{
		public ISagaPolicy<TSaga, TMessage> GetPolicy<TSaga, TMessage>(IEnumerable<State> states)
			where TSaga : SagaStateMachine<TSaga>, ISaga
			where TMessage : class
		{
			bool includesInitial = false;
			bool includesOther = false;

			foreach (var state in states)
			{
				if (IsInitial<TSaga>(state))
					includesInitial = true;
				else
					includesOther = true;
			}

			if (includesInitial && includesOther)
				return new CreateOrUseExistingSagaPolicy<TSaga, TMessage>();

			if (includesInitial)
				return new InitiatingSagaPolicy<TSaga, TMessage>();

			return new ExistingOrIgnoreSagaPolicy<TSaga, TMessage>();
		}

		private bool IsInitial<TSaga>(State state)
			where TSaga : SagaStateMachine<TSaga>, ISaga
		{
			return state.Name == "Initial";
		}
	}
}