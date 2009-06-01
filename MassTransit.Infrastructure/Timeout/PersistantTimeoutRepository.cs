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
namespace MassTransit.Infrastructure.Timeout
{
	using System.Linq;
	using Magnum.Data;
    using Services.Timeout;

	public class PersistantTimeoutRepository :
        ITimeoutRepository
    {
        private readonly IRepository<ScheduledTimeout> _repository;

        public PersistantTimeoutRepository(IRepository<ScheduledTimeout> repository)
        {
            _repository = repository;
        }

        public void Schedule(ScheduledTimeout timeout)
        {
            _repository.Save(timeout);
        }

        public void Remove(ScheduledTimeout timeout)
        {
        	var existing = _repository
        		.Where(x => x.Id == timeout.Id && x.Tag == timeout.Tag)
        		.FirstOrDefault();

			if(existing != null)
			{
				_repository.Delete(existing);	
			}
        }

		public bool TryGetNextScheduledTimeout(out ScheduledTimeout timeout)
		{
			timeout = _repository.OrderBy(x => x.ExpiresAt).FirstOrDefault();

			return timeout != null;
		}

    	public void Dispose()
    	{
    		_repository.Dispose();
    	}
    }
}