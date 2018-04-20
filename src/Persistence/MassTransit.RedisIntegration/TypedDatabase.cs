// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.RedisIntegration
{
    using System;
    using System.Threading.Tasks;
    using StackExchange.Redis;


    public class TypedDatabase<T> :
        ITypedDatabase<T>
        where T : class
    {
        readonly IDatabase _db;

        public TypedDatabase(IDatabase db)
        {
            _db = db;
        }

        public async Task<T> Get(Guid key)
        {
            var value = await _db.StringGetAsync(DatabaseExtensions.FormatSagaKey(key)).ConfigureAwait(false);

            return value.IsNullOrEmpty ? null : SagaSerializer.Deserialize<T>(value);
        }

        public Task Put(Guid key, T value)
        {
            return _db.StringSetAsync(DatabaseExtensions.FormatSagaKey(key), SagaSerializer.Serialize(value));
        }

        public Task Delete(Guid key)
        {
            return _db.KeyDeleteAsync(DatabaseExtensions.FormatSagaKey(key));
        }
    }
}