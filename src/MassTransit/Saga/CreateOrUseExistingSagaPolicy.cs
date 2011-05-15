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
	using System.Linq.Expressions;
	using Magnum;
	using Magnum.Reflection;
	using Util;

	public class CreateOrUseExistingSagaPolicy<TSaga, TMessage> :
		ISagaPolicy<TSaga, TMessage>
		where TSaga : class, ISaga
	{
		private readonly bool _useMessageIdForSagaId;
		private readonly Func<TSaga, bool> _shouldBeRemoved;

		public CreateOrUseExistingSagaPolicy(Expression<Func<TSaga, bool>> shouldBeRemoved)
		{
			_useMessageIdForSagaId = typeof (TMessage).GetInterfaces().Where(x => x == typeof (CorrelatedBy<Guid>)).Any();
			_shouldBeRemoved = shouldBeRemoved.Compile();
		}

		public bool CreateSagaWhenMissing(TMessage message, out TSaga saga)
		{
			Guid sagaId;
			if (!UseMessageIdForSaga(message, out sagaId))
			{
				if (!GenerateNewIdForSaga(out sagaId))
					throw new InvalidOperationException("Could not generate id for new saga " + typeof (TSaga).Name);
			}

			saga = FastActivator<TSaga>.Create(sagaId);

			return saga != null;
		}

		public void ForExistingSaga(TMessage message)
		{
			// do nothing, we are fine if we have an existing saga
		}

		public void ForMissingSaga(TMessage message)
		{
			// do nothing, we dont' care if the saga is missing
		}

		public bool ShouldSagaBeRemoved(TSaga saga)
		{
			return _shouldBeRemoved(saga);
		}

		private bool UseMessageIdForSaga(TMessage message, out Guid sagaId)
		{
			if (_useMessageIdForSagaId)
			{
				var correlator = message.TranslateTo<CorrelatedBy<Guid>>();

				sagaId = correlator.CorrelationId;

				return true;
			}

			sagaId = Guid.Empty;
			return false;
		}

		private static bool GenerateNewIdForSaga(out Guid sagaId)
		{
			sagaId = CombGuid.Generate();
			return true;
		}
	}
}