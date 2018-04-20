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
    using StackExchange.Redis;


    public static class DatabaseExtensions
    {
        public const string SagaPrefix = "saga:";
        public const string SagaLockSuffix = "_lock";

        static readonly Random _random = new Random();

        public static string FormatSagaKey(Guid key)
        {
            return $"{SagaPrefix}{key}";
        }

        public static ITypedDatabase<T> As<T>(this IDatabase db)
            where T : class
        {
            return new TypedDatabase<T>(db);
        }

        public static Task<IAsyncDisposable> AcquireLockAsync(this IDatabase db, Guid sagaId, TimeSpan? expiry = null, TimeSpan? retryTimeout = null)
        {
            return AcquireLockAsync(db, sagaId.ToString(), expiry, retryTimeout);
        }

        static Task<IAsyncDisposable> AcquireLockAsync(this IDatabase db, string sagaId, TimeSpan? expiry = null, TimeSpan? retryTimeout = null)
        {
            if (db == null)
                throw new ArgumentNullException(nameof(db));

            if (sagaId == null)
                throw new ArgumentNullException(nameof(sagaId));

            return DataCacheLock.AcquireAsync(db, sagaId, expiry, retryTimeout);
        }

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
                await Task.Delay(waitFor);
            }

            throw new TimeoutException($"Exceeded timeout of {retryTimeout.Value}");
        }


        class DataCacheLock :
            IAsyncDisposable
        {
            static IDatabase _db;
            readonly TimeSpan? _expiry;
            readonly RedisKey _key;
            readonly RedisValue _value;

            DataCacheLock(IDatabase db, string sagaId, TimeSpan? expiry)
            {
                _db = db;
                _key = $"{SagaPrefix}{sagaId}{SagaLockSuffix}";
                _value = Guid.NewGuid().ToString();
                _expiry = expiry;
            }

            public Task DisposeAsync(CancellationToken cancellationToken)
            {
                return _db.LockReleaseAsync(_key, _value);
            }

            public static Task<IAsyncDisposable> AcquireAsync(IDatabase db, string sagaId, TimeSpan? expiry, TimeSpan? retryTimeout)
            {
                var dataCacheLock = new DataCacheLock(db, sagaId, expiry);

                async Task<IAsyncDisposable> TakeLock()
                {
                    try
                    {
                        var result = await _db.LockTakeAsync(dataCacheLock._key, dataCacheLock._value, dataCacheLock._expiry ?? TimeSpan.MaxValue)
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