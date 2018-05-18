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
namespace MassTransit.ActiveMqTransport.Transport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using Logging;


    public class SessionContextFactory :
        IPipeContextFactory<SessionContext>
    {
        static readonly ILog _log = Logger.Get<SessionContextFactory>();
        readonly IConnectionCache _connectionCache;
        readonly IActiveMqHost _host;

        public SessionContextFactory(IConnectionCache connectionCache, IActiveMqHost host)
        {
            _connectionCache = connectionCache;
            _host = host;
        }

        IPipeContextAgent<SessionContext> IPipeContextFactory<SessionContext>.CreateContext(ISupervisor supervisor)
        {
            IAsyncPipeContextAgent<SessionContext> asyncContext = supervisor.AddAsyncContext<SessionContext>();

            CreateSession(asyncContext, supervisor.Stopped);

            return asyncContext;
        }

        IActivePipeContextAgent<SessionContext> IPipeContextFactory<SessionContext>.CreateActiveContext(ISupervisor supervisor,
            PipeContextHandle<SessionContext> context, CancellationToken cancellationToken)
        {
            return supervisor.AddActiveContext(context, CreateSharedSession(context.Context, cancellationToken));
        }

        async Task<SessionContext> CreateSharedSession(Task<SessionContext> context, CancellationToken cancellationToken)
        {
            var sessionContext = await context.ConfigureAwait(false);

            return new SharedSessionContext(sessionContext, cancellationToken);
        }

        void CreateSession(IAsyncPipeContextAgent<SessionContext> asyncContext, CancellationToken cancellationToken)
        {
            IPipe<ConnectionContext> connectionPipe = Pipe.ExecuteAsync<ConnectionContext>(async connectionContext =>
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Creating session: {0}", connectionContext.Description);

                try
                {
                    var session = await connectionContext.CreateSession().ConfigureAwait(false);

                    var sessionContext = new ActiveMqSessionContext(connectionContext, session, _host, cancellationToken);

                    void HandleException(Exception exception)
                    {
                        var disposeAsync = sessionContext.DisposeAsync(CancellationToken.None);
                    }

                    connectionContext.Connection.ExceptionListener += HandleException;

                    asyncContext.Completed.ContinueWith(task =>
                    {
                        connectionContext.Connection.ExceptionListener -= HandleException;
                    });

                    await asyncContext.Created(sessionContext).ConfigureAwait(false);

                    await asyncContext.Completed.ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    await asyncContext.CreateCanceled().ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    await asyncContext.CreateFaulted(exception).ConfigureAwait(false);
                }
            });

            var connectionTask = _connectionCache.Send(connectionPipe, cancellationToken);

            Task NotifyCreateCanceled(Task task) => asyncContext.CreateCanceled();

            connectionTask.ContinueWith(NotifyCreateCanceled, TaskContinuationOptions.OnlyOnCanceled);

            Task NotifyCreateFaulted(Task task) => asyncContext.CreateFaulted(task.Exception);

            connectionTask.ContinueWith(NotifyCreateFaulted, TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}