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
    /// An active reference to a pipe context, which is managed by an existing pipe context handle.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class ActivePipeContext<TContext> :
        ActivePipeContextHandle<TContext>
        where TContext : class, PipeContext
    {
        readonly PipeContextHandle<TContext> _contextHandle;
        readonly Task<TContext> _context;

        /// <summary>
        /// Creates the active pipe context handle, which must have completed before this instance is created. Otherwise,
        /// it would create a pretty nasty async mess that wouldn't handle faults very well (actually, it should, but I haven't tested it).
        /// </summary>
        /// <param name="contextHandle">The context handle of the actual context which is being used</param>
        /// <param name="context">The actual context, which should be a completed Task</param>
        public ActivePipeContext(PipeContextHandle<TContext> contextHandle, Task<TContext> context)
        {
            _contextHandle = contextHandle;
            _context = context;
        }

        /// <summary>
        /// Creates the active pipe context handle, which must have completed before this instance is created. Otherwise,
        /// it would create a pretty nasty async mess that wouldn't handle faults very well (actually, it should, but I haven't tested it).
        /// </summary>
        /// <param name="contextHandle">The context handle of the actual context which is being used</param>
        /// <param name="context">The actual context</param>
        public ActivePipeContext(PipeContextHandle<TContext> contextHandle, TContext context)
        {
            _contextHandle = contextHandle;
            _context = Task.FromResult(context);
        }

        bool PipeContextHandle<TContext>.IsDisposed => _contextHandle.IsDisposed;

        Task<TContext> PipeContextHandle<TContext>.Context => _context;

        Task ActivePipeContextHandle<TContext>.Faulted(Exception exception)
        {
            return _contextHandle.DisposeAsync();
        }

        Task IAsyncDisposable.DisposeAsync(CancellationToken cancellationToken)
        {
            // An active usage doesn't actually dispose the actual context
            return TaskUtil.Completed;
        }
    }
}