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
namespace MassTransit.MongoDbIntegration.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using CollectionNameFormatters;
    using MassTransit.Saga;
    using MongoDB.Driver;


    public class MongoDbQuerySagaRepository<TSaga> :
        IQuerySagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        readonly IMongoCollection<TSaga> _collection;

        public MongoDbQuerySagaRepository(string connectionString, string database, string collectionName = null)
            : this(new MongoClient(connectionString).GetDatabase(database), collectionName)
        {
        }

        public MongoDbQuerySagaRepository(MongoUrl mongoUrl, string collectionName = null)
            : this(mongoUrl.Url, mongoUrl.DatabaseName, collectionName)
        {
        }

        public MongoDbQuerySagaRepository(IMongoDatabase mongoDatabase, string collectionName = null)
            : this(mongoDatabase, new DefaultCollectionNameFormatter(collectionName))
        {
        }

        public MongoDbQuerySagaRepository(
            IMongoDatabase database,
            ICollectionNameFormatter collectionNameFormatter
        )
        {
            _collection = database.GetCollection<TSaga>(collectionNameFormatter);
        }

        public async Task<IEnumerable<Guid>> Find(ISagaQuery<TSaga> query)
        {
            return await _collection.Find(query.FilterExpression)
                .Project(x => x.CorrelationId)
                .ToListAsync()
                .ConfigureAwait(false);
        }
    }
}
