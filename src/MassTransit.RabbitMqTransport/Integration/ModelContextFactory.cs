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
namespace MassTransit.RabbitMqTransport.Integration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using RabbitMQ.Client;


    public class ModelContextFactory :
        IPipeContextFactory<ModelContext>
    {
        readonly IConnectionContextSupervisor _connectionContextSupervisor;

        public ModelContextFactory(IConnectionContextSupervisor connectionContextSupervisor)
        {
            _connectionContextSupervisor = connectionContextSupervisor;
        }

        IPipeContextAgent<ModelContext> IPipeContextFactory<ModelContext>.CreateContext(ISupervisor supervisor)
        {
            IAsyncPipeContextAgent<ModelContext> asyncContext = supervisor.AddAsyncContext<ModelContext>();

            var context = CreateModel(asyncContext, supervisor.Stopped);

            void HandleShutdown(object sender, ShutdownEventArgs args)
            {
                if (args.Initiator != ShutdownInitiator.Application)
                    asyncContext.Stop(args.ReplyText);
            }

            context.ContinueWith(task =>
            {
                task.Result.Model.ModelShutdown += HandleShutdown;

                asyncContext.Completed.ContinueWith(_ => task.Result.Model.ModelShutdown -= HandleShutdown);
            }, TaskContinuationOptions.OnlyOnRanToCompletion);

            return asyncContext;
        }

        IActivePipeContextAgent<ModelContext> IPipeContextFactory<ModelContext>.CreateActiveContext(ISupervisor supervisor,
            PipeContextHandle<ModelContext> context, CancellationToken cancellationToken)
        {
            return supervisor.AddActiveContext(context, CreateSharedModel(context.Context, cancellationToken));
        }

        async Task<ModelContext> CreateSharedModel(Task<ModelContext> context, CancellationToken cancellationToken)
        {
            var modelContext = await context.ConfigureAwait(false);

            var sharedModel = new SharedModelContext(modelContext, cancellationToken);

            return sharedModel;
        }

        async Task<ModelContext> CreateModel(IAsyncPipeContextAgent<ModelContext> asyncContext, CancellationToken cancellationToken)
        {
            var createModelPipe = new CreateModelPipe(asyncContext, cancellationToken);

            var connectionTask = _connectionContextSupervisor.Send(createModelPipe, cancellationToken);

            await Task.WhenAny(connectionTask, asyncContext.Context).ConfigureAwait(false);
            if (connectionTask.IsFaulted)
                await asyncContext.CreateFaulted(connectionTask.Exception).ConfigureAwait(false);
            else if (connectionTask.IsCanceled)
                await asyncContext.CreateCanceled().ConfigureAwait(false);

            return await asyncContext.Context.ConfigureAwait(false);
        }


        class CreateModelPipe :
            IPipe<ConnectionContext>
        {
            readonly IAsyncPipeContextAgent<ModelContext> _asyncContext;
            readonly CancellationToken _cancellationToken;

            public CreateModelPipe(IAsyncPipeContextAgent<ModelContext> asyncContext, CancellationToken cancellationToken)
            {
                _asyncContext = asyncContext;
                _cancellationToken = cancellationToken;
            }

            public async Task Send(ConnectionContext context)
            {
                try
                {
                    var modelContext = await context.CreateModelContext(_cancellationToken).ConfigureAwait(false);

                    LogContext.Debug?.Log("Created model: {ChannelNumber} {Host}", modelContext.Model.ChannelNumber, context.Description);

                    await _asyncContext.Created(modelContext).ConfigureAwait(false);

                    await _asyncContext.Completed.ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    await _asyncContext.CreateCanceled().ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    await _asyncContext.CreateFaulted(exception).ConfigureAwait(false);
                }
            }

            public void Probe(ProbeContext context)
            {
                context.CreateFilterScope("createModel");
            }
        }
    }
}
