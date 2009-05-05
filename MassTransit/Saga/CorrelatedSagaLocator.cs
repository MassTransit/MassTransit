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
	using System.Linq;
	using Exceptions;
	using log4net;
	using Magnum.Data;

	public class CorrelatedSagaLocator<TSaga, TMessage> :
		ISagaLocator<TSaga, TMessage>
		where TSaga : class, CorrelatedBy<Guid>
		where TMessage : CorrelatedBy<Guid>
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (CorrelatedSagaLocator<TSaga, TMessage>).ToFriendlyName());

		private readonly ISagaPolicy<TSaga, TMessage> _policy;
		private readonly IRepository<TSaga> _repository;

		public CorrelatedSagaLocator(IRepository<TSaga> repository, ISagaPolicy<TSaga, TMessage> policy)
		{
			_repository = repository;
			_policy = policy;
		}

		public bool TryGetSagaForMessage(TMessage message, out InstanceScope<TSaga> instanceScope)
		{
			instanceScope = default(InstanceScope<TSaga>);

			TSaga saga = _repository.Where(x => x.CorrelationId == message.CorrelationId)
				.SingleOrDefault();

			if (saga == null)
			{
				if (!_policy.CreateSagaWhenMissing(message, out saga))
					return false;

				if (_log.IsDebugEnabled)
					_log.DebugFormat("Created saga [{0}] - {1}", typeof (TSaga).ToFriendlyName(), message.CorrelationId);
			}

			instanceScope = new SagaInstanceScope<TSaga>(saga, s =>
				{
					_repository.Update(s);
				});

			return true;
		}
	}
}