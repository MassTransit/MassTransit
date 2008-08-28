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
namespace MassTransit.Saga.Tests.RegisterUser
{
    using System;

    public class RegisterUserSagaRepository :
        ISagaRepository<RegisterUserSaga>
    {
        private readonly IRepository<RegisterUserSaga> _repository;

        public RegisterUserSagaRepository(IRepository<RegisterUserSaga> repository)
        {
            _repository = repository;
        }

        public RegisterUserSaga Create(Guid correlationId)
        {
            return new RegisterUserSaga(correlationId);
        }

        public RegisterUserSaga Get(Guid id)
        {
            return _repository.Get(id);
        }

        public void Save(RegisterUserSaga item)
        {
            _repository.Save(item);
        }

        public void Update(RegisterUserSaga item)
        {
            _repository.Update(item);
        }

        public void Complete(RegisterUserSaga item)
        {
            _repository.Save(item);
            // TODO what do we do when it is time to complete the saga -- you decide to delete or retain
        }
    }
}