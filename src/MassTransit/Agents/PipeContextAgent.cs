// Copyright 2012-2018 Chris Patterson
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
namespace GreenPipes.Agents
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// A PipeContext, which as an agent can be Stopped, which disposes of the context making it unavailable
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class PipeContextAgent<TContext> :
        Agent,
        IPipeContextAgent<TContext>
        where TContext : class, PipeContext
    {
        static readonly string Caption = $"Context<{typeof(TContext).Name}>";

        readonly Task<TContext> _context;
        readonly TaskCompletionSource<DateTime> _inactive;

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        public PipeContextAgent(TContext context)
            : this(Task.FromResult(context))
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        public PipeContextAgent(Task<TContext> context)
        {
            _context = context;
            _inactive = new TaskCompletionSource<DateTime>();

            SetReady(_context);
        }

        bool PipeContextHandle<TContext>.IsDisposed => _inactive.Task.IsCompleted;

        Task<TContext> PipeContextHandle<TContext>.Context => _context;

        /// <inheritdoc />
        public async Task DisposeAsync(CancellationToken cancellationToken)
        {
            // dispose only once
            if (!_inactive.TrySetResult(DateTime.UtcNow))
                return;

            if (_context.Status == TaskStatus.RanToCompletion)
            {
                if (_context.Result is IAsyncDisposable asyncDisposable)
                    await asyncDisposable.DisposeAsync(cancellationToken).ConfigureAwait(false);

                if (_context.Result is IDisposable disposable)
                    disposable.Dispose();
            }

            SetCompleted(_inactive.Task);
        }

        /// <inheritdoc />
        protected override Task StopAgent(StopContext context)
        {
            return DisposeAsync(context.CancellationToken);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Caption;
        }
    }
}