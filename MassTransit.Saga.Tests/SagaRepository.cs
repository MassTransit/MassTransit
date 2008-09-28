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
    using System;
    using Magnum.Common.Repository;

    public class SagaRepository<T> :
        ISagaRepository<T>
        where T : class, IAggregateRoot<Guid>
    {
        private readonly IRepositoryFactory _repositoryFactory;

        public SagaRepository(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public T Create(Guid correlationId)
        {
            T saga = (T) Activator.CreateInstance(typeof (T), correlationId);

            WithRepository(r => r.Save(saga));

            return saga;
        }

        public T Get(Guid id)
        {
            using (IRepository<T, Guid> repository = _repositoryFactory.GetRepository<T, Guid>())
            {
                return repository.Get(id);
            }
        }

        public void Save(T item)
        {
            WithRepository(r => r.Update(item));
        }

        public void Dispose()
        {
        }

        private void WithRepository(Action<IRepository<T, Guid>> action)
        {
            using (IRepository<T, Guid> repository = _repositoryFactory.GetRepository<T, Guid>())
            {
                action(repository);
            }
        }
    }
}