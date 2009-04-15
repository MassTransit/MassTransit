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
	using System.Linq;
	using MassTransit.Saga;

	public class CorrelatedSagaLocator<TSaga, TMessage> :
		ISagaLocator<TSaga, TMessage>
		where TSaga : class, CorrelatedBy<Guid>
		where TMessage : CorrelatedBy<Guid>
	{
		private readonly ISagaRepository<TSaga> _repository;

		public CorrelatedSagaLocator(ISagaRepository<TSaga> repository)
		{
			_repository = repository;
		}

		public TSaga GetSagaForMessage(TMessage message)
		{
			return _repository.Where(x => x.CorrelationId == message.CorrelationId).FirstOrDefault();
		}
	}
}