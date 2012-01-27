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
namespace MassTransit.Saga
{
	using System;
	using System.Linq.Expressions;
	using Exceptions;
	using Logging;

    public class ExistingOrIgnoreSagaPolicy<TSaga, TMessage> :
		ISagaPolicy<TSaga, TMessage>
		where TSaga : class, ISaga
		where TMessage : class
	{
		static readonly ILog _log = Logger.Get("MassTransit.Saga.ExistingOrIgnoreSagaPolicy");

		private readonly Func<TSaga, bool> _canRemoveInstance;

		public ExistingOrIgnoreSagaPolicy(Expression<Func<TSaga, bool>> shouldBeRemoved)
		{
			_canRemoveInstance = shouldBeRemoved.Compile();
		}

		public bool CanCreateInstance(IConsumeContext<TMessage> context)
		{
			return false;
		}

		public TSaga CreateInstance(IConsumeContext<TMessage> context, Guid sagaId)
		{
			throw new SagaException("The policy does not allow saga creation", typeof (TSaga), typeof (TMessage));
		}

		public Guid GetNewSagaId(IConsumeContext<TMessage> context)
		{
			throw new SagaException("The policy does not allow saga creation", typeof(TSaga), typeof(TMessage));
		}

		public bool CanUseExistingInstance(IConsumeContext<TMessage> context)
		{
			return true;
		}

		public bool CanRemoveInstance(TSaga instance)
		{
			return _canRemoveInstance(instance);
		}
	}
}