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
namespace MassTransit.Pipeline.Filters.ConcurrencyLimit
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Contracts;
    using Pipes;


    /// <summary>
    /// Limits the concurrency of the next section of the pipeline based on the concurrency limit
    /// specified.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConcurrencyLimitFilter<T> :
        IFilter<T>,
        IConsumer<SetConcurrencyLimit>,
        IDisposable
        where T : class, PipeContext
    {
        readonly int _concurrencyLimit;
        readonly ConnectHandle _handle;
        readonly SemaphoreSlim _limit;

        public ConcurrencyLimitFilter(int concurrencyLimit, IManagementPipe managementPipe)
        {
            _concurrencyLimit = concurrencyLimit;

            _limit = new SemaphoreSlim(concurrencyLimit);

            _handle = managementPipe.ConnectInstance(this);
        }

        public async Task Consume(ConsumeContext<SetConcurrencyLimit> context)
        {
            var concurrencyLimit = context.Message.ConcurrencyLimit;
            if (concurrencyLimit < 1)
                throw new ArgumentOutOfRangeException(nameof(concurrencyLimit), "The concurrency limit must be >= 1");

            var previousLimit = _concurrencyLimit;
            if (concurrencyLimit > previousLimit)
                _limit.Release(concurrencyLimit - previousLimit);
            else
            {
                for (; previousLimit > concurrencyLimit; previousLimit--)
                    await _limit.WaitAsync().ConfigureAwait(false);
            }
        }

        public void Dispose()
        {
            _limit?.Dispose();
            _handle?.Dispose();
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("concurrencyLimit");
            scope.Add("limit", _concurrencyLimit);
            scope.Add("available", _limit.CurrentCount);
        }

        [DebuggerNonUserCode]
        public async Task Send(T context, IPipe<T> next)
        {
            await _limit.WaitAsync(context.CancellationToken).ConfigureAwait(false);

            try
            {
                await next.Send(context).ConfigureAwait(false);
            }
            finally
            {
                _limit.Release();
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