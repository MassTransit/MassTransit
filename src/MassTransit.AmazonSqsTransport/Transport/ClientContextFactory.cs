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
namespace MassTransit.AmazonSqsTransport.Transport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using Logging;


    public class ClientContextFactory :
        IPipeContextFactory<ClientContext>
    {
        readonly IConnectionContextSupervisor _connectionContextSupervisor;

        public ClientContextFactory(IConnectionContextSupervisor connectionContextSupervisor)
        {
            _connectionContextSupervisor = connectionContextSupervisor;
        }

        IPipeContextAgent<ClientContext> IPipeContextFactory<ClientContext>.CreateContext(ISupervisor supervisor)
        {
            IAsyncPipeContextAgent<ClientContext> asyncContext = supervisor.AddAsyncContext<ClientContext>();

            CreateModel(asyncContext, supervisor.Stopped);

            return asyncContext;
        }

        IActivePipeContextAgent<ClientContext> IPipeContextFactory<ClientContext>.CreateActiveContext(ISupervisor supervisor,
            PipeContextHandle<ClientContext> context, CancellationToken cancellationToken)
        {
            return supervisor.AddActiveContext(context, CreateSharedSession(context.Context, cancellationToken));
        }

        async Task<ClientContext> CreateSharedSession(Task<ClientContext> context, CancellationToken cancellationToken)
        {
            var modelContext = await context.ConfigureAwait(false);

            return new SharedClientContext(modelContext, cancellationToken);
        }

        void CreateModel(IAsyncPipeContextAgent<ClientContext> asyncContext, CancellationToken cancellationToken)
        {
            IPipe<ConnectionContext> connectionPipe = Pipe.ExecuteAsync<ConnectionContext>(async connectionContext =>
            {
                try
                {
                    var amazonSqs = await connectionContext.CreateAmazonSqs().ConfigureAwait(false);
                    var amazonSns = await connectionContext.CreateAmazonSns().ConfigureAwait(false);

                    var modelContext = new AmazonSqsClientContext(connectionContext, amazonSqs, amazonSns, cancellationToken);

                    await asyncContext.Created(modelContext).ConfigureAwait(false);

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

            var connectionTask = _connectionContextSupervisor.Send(connectionPipe, cancellationToken);

            Task NotifyCreateCanceled(Task task) => asyncContext.CreateCanceled();

            connectionTask.ContinueWith(NotifyCreateCanceled, TaskContinuationOptions.OnlyOnCanceled);

            Task NotifyCreateFaulted(Task task) => asyncContext.CreateFaulted(task.Exception);

            connectionTask.ContinueWith(NotifyCreateFaulted, TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}
