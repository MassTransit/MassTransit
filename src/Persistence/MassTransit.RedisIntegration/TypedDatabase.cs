// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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


    public class TypedDatabase<T> : ITypedDatabase<T> where T : class
    {
        readonly IDatabase _db;

        public TypedDatabase(IDatabase db) => _db = db;

        public async Task<T> Get(Guid key)
        {
            var value = await _db.StringGetAsync(DatabaseExtensions.SagaPrefix + key.ToString()).ConfigureAwait(false);
            return value.IsNullOrEmpty ? null : SagaSerializer.Deserialize<T>(value);
        }

        public async Task Put(Guid key, T value) =>
            await _db.StringSetAsync(DatabaseExtensions.SagaPrefix + key.ToString(), SagaSerializer.Serialize(value)).ConfigureAwait(false);

        public async Task Delete(Guid key) => await _db.KeyDeleteAsync(DatabaseExtensions.SagaPrefix + key.ToString()).ConfigureAwait(false);
    }

    public interface ITypedDatabase<T> where T : class
    {
        Task<T> Get(Guid key);
        Task Put(Guid key, T value);
        Task Delete(Guid key);
    }

    public static class DatabaseExtensions
    {
        public const string SagaPrefix = "saga:";
        public const string SagaLockSuffix = "_lock";

        public static ITypedDatabase<T> As<T>(this IDatabase db) where T : class =>
            new TypedDatabase<T>(db);

        public static Task<IDisposable> AcquireLockAsync(this IDatabase db, Guid sagaId, TimeSpan? expiry = null, TimeSpan? retryTimeout = null)
        {
            return AcquireLockAsync(db, sagaId.ToString(), expiry, retryTimeout = null);
        }

        public static Task<IDisposable> AcquireLockAsync(this IDatabase db, string sagaId, TimeSpan? expiry = null, TimeSpan? retryTimeout = null)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            if (sagaId == null)
            {
                throw new ArgumentNullException("sagaId");
            }

            return DataCacheLock.AcquireAsync(db, sagaId, expiry, retryTimeout);
        }

        private class DataCacheLock : IDisposable
        {
            private static StackExchange.Redis.IDatabase _db;
            public readonly RedisKey Key;
            public readonly RedisValue Value;
            public readonly TimeSpan? Expiry;

            private DataCacheLock(IDatabase db, string sagaId, TimeSpan? expiry)
            {
                _db = db;
                Key = $"{DatabaseExtensions.SagaPrefix}{sagaId}{DatabaseExtensions.SagaLockSuffix}";
                Value = Guid.NewGuid().ToString();
                Expiry = expiry;
            }

            public static async Task<IDisposable> AcquireAsync(IDatabase db, string sagaId, TimeSpan? expiry, TimeSpan? retryTimeout)
            {
                DataCacheLock dataCacheLock = new DataCacheLock(db, sagaId, expiry);
                Func<Task<bool>> task = async () =>
                {
                    try
                    {
                        return await _db.LockTakeAsync(dataCacheLock.Key, dataCacheLock.Value, dataCacheLock.Expiry ?? TimeSpan.MaxValue);
                    }
                    catch
                    {
                        return false;
                    }
                };

                await RetryUntilTrueAsync(task, retryTimeout);
                return dataCacheLock;
            }
            public void Dispose()
            {
                _db.LockReleaseAsync(Key, Value).Wait();
            }
        }

        private static readonly Random _random = new Random();

        private static async Task<bool> RetryUntilTrueAsync(Func<Task<bool>> task, TimeSpan? retryTimeout)
        {
            int i = 0;
            DateTime utcNow = DateTime.UtcNow;
            while (!retryTimeout.HasValue || DateTime.UtcNow - utcNow < retryTimeout.Value)
            {
                i++;
                if (await task())
                {
                    return true;
                }
                var waitFor = _random.Next((int)Math.Pow(i, 2), (int)Math.Pow(i + 1, 2) + 1);
                await Task.Delay(waitFor);
            }

            throw new TimeoutException(string.Format("Exceeded timeout of {0}", retryTimeout.Value));
        }
    }
}