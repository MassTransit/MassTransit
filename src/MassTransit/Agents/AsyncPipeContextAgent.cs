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
    /// A PipeContext, which as an agent can be Stopped, which disposes of the context making it unavailable
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class AsyncPipeContextAgent<TContext> :
        IAsyncPipeContextAgent<TContext>
        where TContext : class, PipeContext
    {
        readonly IPipeContextAgent<TContext> _agent;
        readonly TaskCompletionSource<TContext> _context;

        public AsyncPipeContextAgent()
        {
            _context = new TaskCompletionSource<TContext>();

            _agent = new PipeContextAgent<TContext>(_context.Task);
        }

        bool PipeContextHandle<TContext>.IsDisposed => _agent.IsDisposed;

        Task<TContext> PipeContextHandle<TContext>.Context => _agent.Context;

        Task IAsyncDisposable.DisposeAsync(CancellationToken cancellationToken)
        {
            return _agent.DisposeAsync(cancellationToken);
        }

        Task IAgent.Ready => _agent.Ready;
        Task IAgent.Completed => _agent.Completed;

        CancellationToken IAgent.Stopping => _agent.Stopping;
        CancellationToken IAgent.Stopped => _agent.Stopped;

        Task IAgent.Stop(StopContext context)
        {
            return _agent.Stop(context);
        }

        Task IAsyncPipeContextHandle<TContext>.Created(TContext context)
        {
            _context.SetResult(context);

            return TaskUtil.Completed;
        }

        Task IAsyncPipeContextHandle<TContext>.CreateCanceled()
        {
            _context.SetCanceled();

            return _agent.Stop("Create Canceled", CancellationToken.None);
        }

        Task IAsyncPipeContextHandle<TContext>.CreateFaulted(Exception exception)
        {
            _context.SetException(exception);

            return _agent.Stop($"Create Faulted: {exception.GetBaseException().Message}", CancellationToken.None);
        }

        Task IAsyncPipeContextHandle<TContext>.Faulted(Exception exception)
        {
            return _agent.Stop($"Faulted: {exception.GetBaseException().Message}", CancellationToken.None);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return _agent.ToString();
        }
    }
}