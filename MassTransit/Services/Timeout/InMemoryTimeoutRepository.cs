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
namespace MassTransit.Services.Timeout
{
	using System;
	using System.Linq;
	using Magnum.Data;

	public class InMemoryTimeoutRepository :
		InMemoryRepository<ScheduledTimeout, Guid>,
		ITimeoutRepository
	{
		private readonly object _lock = new object();

		public InMemoryTimeoutRepository()
			: base(x => x.Id)
		{
		}

		public void Schedule(ScheduledTimeout timeout)
		{
			lock (_lock)
			{
				ScheduledTimeout existing = this.Where(x => x.Id == timeout.Id && x.ExpiresAt == timeout.ExpiresAt).FirstOrDefault();
				if (existing == null)
				{
					Save(timeout);
				}
				else
				{
					existing.ExpiresAt = timeout.ExpiresAt;
				}
			}
		}

		public void Remove(ScheduledTimeout timeout)
		{
			lock (_lock)
			{
				ScheduledTimeout existing = this.Where(x => x.Id == timeout.Id && x.ExpiresAt == timeout.ExpiresAt).FirstOrDefault();
				if (existing != null)
				{
					Delete(existing);
				}
			}
		}

		public bool TryGetNextScheduledTimeout(out ScheduledTimeout timeout)
		{
			timeout = this.OrderBy(x => x.ExpiresAt).FirstOrDefault();

			return timeout != null;
		}
	}
}