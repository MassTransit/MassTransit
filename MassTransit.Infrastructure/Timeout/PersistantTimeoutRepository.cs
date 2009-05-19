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
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
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

    	public IEnumerator<ScheduledTimeout> GetEnumerator()
    	{
    		return _repository.GetEnumerator();
    	}

    	IEnumerator IEnumerable.GetEnumerator()
    	{
    		return GetEnumerator();
    	}

    	public Expression Expression
    	{
    		get { return _repository.Expression; }
    	}

    	public Type ElementType
    	{
			get { return _repository.ElementType; }
    	}

    	public IQueryProvider Provider
    	{
			get { return _repository.Provider; }
    	}

    	public void Dispose()
    	{
    		_repository.Dispose();
    	}

    	public void Save(ScheduledTimeout item)
    	{
    		_repository.Save(item);
    	}

    	public void Update(ScheduledTimeout item)
    	{
    		_repository.Update(item);
    	}

    	public void Delete(ScheduledTimeout item)
    	{
    		_repository.Delete(item);
    	}
    }
}