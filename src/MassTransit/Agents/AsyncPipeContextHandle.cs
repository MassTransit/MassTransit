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
    using Util;


    /// <summary>
    /// An asynchronously pipe context handle, which can be completed.
    /// </summary>
    /// <typeparam name="TContext">The context type</typeparam>
    public class AsyncPipeContextHandle<TContext> :
        IAsyncPipeContextHandle<TContext>
        where TContext : class, PipeContext
    {
        readonly TaskCompletionSource<TContext> _context;
        readonly TaskCompletionSource<DateTime> _inactive;

        /// <summary>
        /// Creates the handle
        /// </summary>
        public AsyncPipeContextHandle()
        {
            _context = new TaskCompletionSource<TContext>();
            _inactive = new TaskCompletionSource<DateTime>();
        }

        bool PipeContextHandle<TContext>.IsDisposed => _inactive.Task.IsCompleted;

        Task<TContext> PipeContextHandle<TContext>.Context => _context.Task;

        Task IAsyncPipeContextHandle<TContext>.Created(TContext context)
        {
            _context.SetResult(context);

            return TaskUtil.Completed;
        }

        Task IAsyncPipeContextHandle<TContext>.CreateCanceled()
        {
            _context.SetCanceled();

            return TaskUtil.Completed;
        }

        Task IAsyncPipeContextHandle<TContext>.CreateFaulted(Exception exception)
        {
            _context.SetException(exception);

            return TaskUtil.Completed;
        }

        Task IAsyncPipeContextHandle<TContext>.Faulted(Exception exception)
        {
            _inactive.TrySetException(exception);

            return TaskUtil.Completed;
        }

        Task IAsyncDisposable.DisposeAsync(CancellationToken cancellationToken)
        {
            _inactive.TrySetResult(DateTime.UtcNow);

            return TaskUtil.Completed;
        }
    }
}