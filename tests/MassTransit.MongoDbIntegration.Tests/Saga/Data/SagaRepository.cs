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
namespace MassTransit.MongoDbIntegration.Tests.Saga
{
    using System;
    using System.Threading.Tasks;
    using MongoDB.Bson;
    using MongoDB.Driver;


    public static class SagaRepository
    {
        public static IMongoDatabase Instance => new MongoClient("mongodb://127.0.0.1").GetDatabase("sagaTest");

        public static Task InsertSaga(SimpleSaga saga)
        {
            return Instance.GetCollection<SimpleSaga>("sagas").InsertOneAsync(saga);
        }

        public static Task DeleteSaga(Guid correlationId)
        {
            return Instance.GetCollection<BsonDocument>("sagas").DeleteOneAsync(x => x["_id"] == correlationId);
        }

        public static Task<SimpleSaga> GetSaga(Guid correlationId)
        {
            return Instance.GetCollection<SimpleSaga>("sagas").Find(x => x.CorrelationId == correlationId).SingleOrDefaultAsync();
        }
    }
}
