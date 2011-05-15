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
	using System.Collections.Generic;
	using Exceptions;
	using log4net;
	using Util;

	public class AbstractSagaRepository<TSaga>
		where TSaga : class, ISaga
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (AbstractSagaRepository<TSaga>));

		protected static bool SendMessageToExistingSagas<TMessage>(IEnumerable<TSaga> existingSagas,
		                                                           ISagaPolicy<TSaga, TMessage> policy,
		                                                           Action<TSaga> consumerAction,
		                                                           TMessage message,
		                                                           Action<TSaga> removeAction)
		{
			int sagaCount = 0;
			Exception lastException = null;

			foreach (TSaga saga in existingSagas)
			{
				try
				{
					sagaCount++;

					if (_log.IsDebugEnabled)
						_log.DebugFormat("Found saga [{0}] - {1}", typeof (TSaga).ToFriendlyName(), saga.CorrelationId);

					policy.ForExistingSaga(message);

					lock (saga)
						consumerAction(saga);

					if (policy.ShouldSagaBeRemoved(saga))
						removeAction(saga);
				}
				catch (Exception ex)
				{
					var sex = new SagaException("Saga consumer exception", typeof (TSaga), typeof (TMessage), saga.CorrelationId, ex);
					if (_log.IsErrorEnabled)
						_log.Error("Existing Saga Exception: ", sex);

					lastException = sex;
				}
			}

			if (lastException != null)
				throw lastException;

			return sagaCount > 0;
		}

		protected static void SendMessageToNewSaga<TMessage>(ISagaPolicy<TSaga, TMessage> policy, TMessage message, Action<TSaga> consumerAction, Action<TSaga> removeAction)
		{
			TSaga saga;
			if (!policy.CreateSagaWhenMissing(message, out saga))
				return;

			if (_log.IsDebugEnabled)
				_log.DebugFormat("Created saga [{0}] - {1}", typeof (TSaga).ToFriendlyName(), saga.CorrelationId);

			try
			{
				lock (saga)
					consumerAction(saga);

				if (policy.ShouldSagaBeRemoved(saga))
					removeAction(saga);
			}
			catch (Exception ex)
			{
				var sex = new SagaException("Saga consumer exception", typeof (TSaga), typeof (TMessage), saga.CorrelationId, ex);
				if (_log.IsErrorEnabled)
					_log.Error("Existing Saga Exception: ", sex);

				throw sex;
			}
		}
	}
}