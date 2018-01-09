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
    /// An Agent Provocateur that uses a context handle for the activate state of the agent
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class ActivePipeContextAgent<TContext> :
        Agent,
        IActivePipeContextAgent<TContext>
        where TContext : class, PipeContext
    {
        static readonly string Caption = $"Active<{typeof(TContext).Name}>";

        readonly ActivePipeContextHandle<TContext> _contextHandle;

        public ActivePipeContextAgent(ActivePipeContextHandle<TContext> context)
        {
            _contextHandle = context;

            context.Context.ContinueWith(SetReady, CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default);
            context.Context.ContinueWith(SetFaulted, CancellationToken.None, TaskContinuationOptions.NotOnRanToCompletion, TaskScheduler.Default);
        }

        /// <inheritdoc />
        protected override async Task StopAgent(StopContext context)
        {
            if (_contextHandle.Context.Status == TaskStatus.RanToCompletion)
                await _contextHandle.DisposeAsync(context.CancellationToken).ConfigureAwait(false);

            SetCompleted(TaskUtil.Completed);
        }

        bool PipeContextHandle<TContext>.IsDisposed => _contextHandle.IsDisposed;

        Task<TContext> PipeContextHandle<TContext>.Context => _contextHandle.Context;

        Task ActivePipeContextHandle<TContext>.Faulted(Exception exception)
        {
            return _contextHandle.Faulted(exception);
        }

        Task IAsyncDisposable.DisposeAsync(CancellationToken cancellationToken)
        {
            return _contextHandle.DisposeAsync(cancellationToken);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Caption;
        }
    }
}