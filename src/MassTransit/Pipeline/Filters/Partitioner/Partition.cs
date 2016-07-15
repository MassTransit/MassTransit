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
namespace MassTransit.Pipeline.Filters.Partitioner
{
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Util;


    public class Partition :
        IPartition,
        IAsyncDisposable
    {
        readonly int _index;
        readonly SemaphoreSlim _limit;
        long _attemptCount;
        long _failureCount;
        long _successCount;

        public Partition(int index)
        {
            _index = index;
            _limit = new SemaphoreSlim(1);
        }

        public async Task DisposeAsync()
        {
            await WaitForRunningTasks(CancellationToken.None).ConfigureAwait(false);

            _limit?.Dispose();
        }

        [DebuggerNonUserCode]
        public async Task Send<T>(T context, IPipe<T> next)
            where T : class, PipeContext
        {
            await _limit.WaitAsync(context.CancellationToken).ConfigureAwait(false);

            try
            {
                Interlocked.Increment(ref _attemptCount);

                await next.Send(context).ConfigureAwait(false);

                Interlocked.Increment(ref _successCount);
            }
            catch
            {
                Interlocked.Increment(ref _failureCount);
                throw;
            }
            finally
            {
                _limit.Release();
            }
        }

        public void Probe(ProbeContext context)
        {
            var partitionScope = context.CreateScope($"partition-{_index}");
            partitionScope.Set(new
            {
                AttemptCount = _attemptCount,
                SuccessCount = _successCount,
                FailureCount = _failureCount
            });
        }

        /// <summary>
        /// A hack, but waits for any tasks that have been sent through the filter to complete by
        /// waiting and taking all the concurrent slots
        /// </summary>
        /// <param name="cancellationToken">Of course we can cancel the operation</param>
        /// <returns></returns>
        async Task WaitForRunningTasks(CancellationToken cancellationToken)
        {
            await _limit.WaitAsync(cancellationToken).ConfigureAwait(false);

            _limit.Release(1);
        }
    }
}