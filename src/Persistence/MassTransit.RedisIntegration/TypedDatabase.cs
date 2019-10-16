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
    using static DatabaseExtensions;


    public class TypedDatabase<T> :
        ITypedDatabase<T>
        where T : class
    {
        readonly IDatabase _db;

        IDatabase ITypedDatabase<T>.Database => _db;

        public TypedDatabase(IDatabase db)
        {
            _db = db;
        }

        async Task<T> ITypedDatabase<T>.Get(Guid key)
        {
            var value = await _db.StringGetAsync(GetKeyWithPrefix(key)).ConfigureAwait(false);

            return value.IsNullOrEmpty ? null : SagaSerializer.Deserialize<T>(value);
        }

        Task ITypedDatabase<T>.Put(Guid key, T value)
        {
            return _db.StringSetAsync(GetKeyWithPrefix(key), SagaSerializer.Serialize(value));
        }

        Task ITypedDatabase<T>.Delete(Guid key)
        {
            return _db.KeyDeleteAsync(GetKeyWithPrefix(key));
        }
    }
}
