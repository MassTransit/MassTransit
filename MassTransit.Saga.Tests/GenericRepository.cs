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
namespace MassTransit.Saga.Tests
{
    public class GenericRepository<T> :
        IRepository<T>
        where T : class
    {
        private readonly IRepository _repository;

        public GenericRepository(IRepository repository)
        {
            _repository = repository;
        }

        public void Dispose()
        {
            _repository.Dispose();
        }

        public T Get(object id)
        {
            return _repository.Get<T>(id);
        }

        public void Save(T item)
        {
            _repository.Save(item);
        }

        public void Update(T item)
        {
            _repository.Save(item);
        }
    }
}