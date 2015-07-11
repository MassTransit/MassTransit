// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Pipeline.Filters
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// Limits the concurrency of the next section of the pipeline based on the concurrency limit
    /// specified.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConcurrencyLimitFilter<T> :
        IFilter<T>,
        IDisposable
        where T : class, PipeContext
    {
        readonly int _concurrencyLimit;
        readonly SemaphoreSlim _limit;

        public ConcurrencyLimitFilter(int concurrencyLimit)
        {
            _concurrencyLimit = concurrencyLimit;
            _limit = new SemaphoreSlim(concurrencyLimit);
        }

        public void Dispose()
        {
            if (_limit != null)
                _limit.Dispose();
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            ProbeContext scope = context.CreateFilterScope("concurrencyLimit");
            scope.Add("limit", _concurrencyLimit);
            scope.Add("available", _limit.CurrentCount);
        }

        [DebuggerNonUserCode]
        public async Task Send(T context, IPipe<T> next)
        {
            await _limit.WaitAsync(context.CancellationToken);

            try
            {
                await next.Send(context);
            }
            finally
            {
                _limit.Release();
            }
        }

        public async Task SetConcurrencyLimit(int concurrencyLimit)
        {
            if (concurrencyLimit < 1)
                throw new ArgumentOutOfRangeException("concurrencyLimit", "The concurrency limit must be >= 1");

            int previousLimit = _concurrencyLimit;
            if (concurrencyLimit > previousLimit)
                _limit.Release(concurrencyLimit - previousLimit);
            else
            {
                for (; previousLimit > concurrencyLimit; previousLimit--)
                    await _limit.WaitAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// A hack, but waits for any tasks that have been sent through the filter to complete by
        /// waiting and taking all the concurrent slots
        /// </summary>
        /// <param name="cancellationToken">Of course we can cancel the operation</param>
        /// <returns></returns>
        async Task WaitForRunningTasks(CancellationToken cancellationToken)
        {
            int slot;
            for (slot = 0; slot < _concurrencyLimit; slot++)
                await _limit.WaitAsync(cancellationToken).ConfigureAwait(false);

            _limit.Release(slot);
        }
    }
}