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
namespace MassTransit.Testing.TestDecorators
{
	using System;
	using Saga;

	public class SagaPolicyTestDecorator<TSaga, TMessage> :
		ISagaPolicy<TSaga, TMessage>
		where TMessage : class
		where TSaga : class, ISaga
	{
		readonly ISagaPolicy<TSaga, TMessage> _policy;
		readonly Guid _sagaId;
		readonly SagaListImpl<TSaga> _created;

		public SagaPolicyTestDecorator(ISagaPolicy<TSaga, TMessage> policy, Guid sagaId, SagaListImpl<TSaga> created)
		{
			_policy = policy;
			_sagaId = sagaId;
			_created = created;
		}

		public bool CanCreateInstance(IConsumeContext<TMessage> context)
		{
			return _policy.CanCreateInstance(context);
		}

		public TSaga CreateInstance(IConsumeContext<TMessage> context, Guid sagaId)
		{
			TSaga instance = _policy.CreateInstance(context, sagaId);
			if(instance != null)
			{
				_created.Add(new SagaInstanceImpl<TSaga>(instance));
			}

			return instance;
		}

		public Guid GetNewSagaId(IConsumeContext<TMessage> context)
		{
			return _policy.GetNewSagaId(context);
		}

		public bool CanUseExistingInstance(IConsumeContext<TMessage> context)
		{
			return _policy.CanUseExistingInstance(context);
		}

		public bool CanRemoveInstance(TSaga instance)
		{
			return _policy.CanRemoveInstance(instance);
		}
	}
}