// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.MongoDbIntegration.Saga.Context
{
    using System.Threading.Tasks;
    using MassTransit.Saga;
    using MongoDB.Driver;


    public class MongoDbSagaConsumeContextFactory<TSaga> :
        ISagaConsumeContextFactory<IMongoCollection<TSaga>, TSaga>
        where TSaga : class, IVersionedSaga
    {
        public Task<SagaConsumeContext<TSaga, T>> CreateSagaConsumeContext<T>(IMongoCollection<TSaga> context, ConsumeContext<T> consumeContext,
            TSaga instance, SagaConsumeContextMode mode)
            where T : class
        {
            return Task.FromResult<SagaConsumeContext<TSaga, T>>(new MongoDbSagaConsumeContext<TSaga, T>(context, consumeContext, instance, mode));
        }
    }
}
