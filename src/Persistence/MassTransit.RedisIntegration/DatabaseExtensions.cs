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
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Metadata;
    using StackExchange.Redis;
    using Util;


    public static class DatabaseExtensions
    {
        const string SagaLockSuffix = "_lock";
        static string _keyPrefix;
        static readonly Random _random = new Random();
        
        public static ITypedDatabase<T> As<T>(this IDatabase db)
            where T : class
        {
            return new TypedDatabase<T>(db);
        }

        internal static Task<IAsyncDisposable> AcquireLockAsync(this IDatabase db, Guid sagaId, TimeSpan? expiry, TimeSpan? retryTimeout) =>
            DataCacheLock.AcquireAsync(db, GetKeyWithPrefix(sagaId), expiry, retryTimeout);

        static async Task<T> RetryUntilTrueAsync<T>(Func<Task<T>> task, TimeSpan? retryTimeout)
        {
            var i = 0;
            var utcNow = DateTime.UtcNow;
            while (!retryTimeout.HasValue || DateTime.UtcNow - utcNow < retryTimeout.Value)
            {
                i++;
                var result = await task().ConfigureAwait(false);
                if (result != null)
                    return result;

                var waitFor = _random.Next((int)Math.Pow(i, 2), (int)Math.Pow(i + 1, 2) + 1);
                await Task.Delay(waitFor).ConfigureAwait(false);
            }

            throw new TimeoutException($"Exceeded timeout of {retryTimeout.Value}");
        }

        internal static void SetKeyPrefix(string keyPrefix) => _keyPrefix = keyPrefix;
        internal static string GetKeyWithPrefix(Guid key) => string.IsNullOrWhiteSpace(_keyPrefix) ? key.ToString() : $"{_keyPrefix}:{key}";

        class DataCacheLock :
            IAsyncDisposable
        {
            readonly IDatabase _db;
            readonly TimeSpan? _expiry;
            readonly RedisKey _key;
            readonly RedisValue _token;

            DataCacheLock(IDatabase db, string sagaId, TimeSpan? expiry)
            {
                _db = db;
                _key = $"{sagaId}{SagaLockSuffix}";
                _token = $"{HostMetadataCache.Host.MachineName}:{NewId.NextGuid()}";
                _expiry = expiry;
            }

            public Task DisposeAsync(CancellationToken cancellationToken)
            {
                return _db.LockReleaseAsync(_key, _token);
            }

            public static Task<IAsyncDisposable> AcquireAsync(IDatabase db, string sagaId, TimeSpan? expiry, TimeSpan? retryTimeout)
            {
                var dataCacheLock = new DataCacheLock(db, sagaId, expiry);

                async Task<IAsyncDisposable> TakeLock()
                {
                    try
                    {
                        var result = await db.LockTakeAsync(dataCacheLock._key, dataCacheLock._token, dataCacheLock._expiry ?? TimeSpan.MaxValue)
                            .ConfigureAwait(false);

                        if (result)
                            return dataCacheLock;
                    }
                    catch
                    {
                    }

                    return null;
                }

                return RetryUntilTrueAsync(TakeLock, retryTimeout);
            }
        }
    }
}
