namespace MassTransit.AmazonSqsTransport.Pipeline
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.SQS.Model;
    using Context;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using GreenPipes.Internals.Extensions;
    using Topology;
    using Transports;
    using Transports.Metrics;
    using Util;


    /// <summary>
    /// Receives messages from AmazonSQS, pushing them to the InboundPipe of the service endpoint.
    /// </summary>
    public sealed class AmazonSqsMessageReceiver :
        Supervisor,
        DeliveryMetrics
    {
        readonly ClientContext _client;
        readonly SqsReceiveEndpointContext _context;
        readonly TaskCompletionSource<bool> _deliveryComplete;
        readonly IReceivePipeDispatcher _dispatcher;
        readonly ReceiveSettings _receiveSettings;

        /// <summary>
        /// The basic consumer receives messages pushed from the broker.
        /// </summary>
        /// <param name="client">The model context for the consumer</param>
        /// <param name="context">The topology</param>
        public AmazonSqsMessageReceiver(ClientContext client, SqsReceiveEndpointContext context)
        {
            _client = client;
            _context = context;

            _receiveSettings = client.GetPayload<ReceiveSettings>();

            _deliveryComplete = TaskUtil.GetTask<bool>();

            _dispatcher = context.CreateReceivePipeDispatcher();
            _dispatcher.ZeroActivity += HandleDeliveryComplete;

            Task.Run(Consume);
        }

        long DeliveryMetrics.DeliveryCount => _dispatcher.DispatchCount;
        int DeliveryMetrics.ConcurrentDeliveryCount => _dispatcher.MaxConcurrentDispatchCount;

        async Task Consume()
        {
            // TODO add ConcurrencyLimit for receive settings
            var executor = new ChannelExecutor(_receiveSettings.PrefetchCount, _receiveSettings.PrefetchCount);

            SetReady();

            try
            {
                await PollMessages(executor).ConfigureAwait(false);
            }
            catch (OperationCanceledException exception) when (exception.CancellationToken == Stopping)
            {
            }
            catch (Exception exception)
            {
                LogContext.Error?.Log(exception, "Consume Loop faulted");
            }
            finally
            {
                await executor.DisposeAsync(CancellationToken.None).ConfigureAwait(false);
            }

            SetCompleted(TaskUtil.Completed);
        }

        protected override async Task StopSupervisor(StopSupervisorContext context)
        {
            LogContext.Debug?.Log("Stopping consumer: {InputAddress}", _context.InputAddress);

            SetCompleted(ActiveAndActualAgentsCompleted(context));

            await Completed.ConfigureAwait(false);
        }

        async Task HandleMessage(Message message)
        {
            if (IsStopping)
                return;

            var redelivered = message.Attributes.TryGetInt("ApproximateReceiveCount", out var receiveCount) && receiveCount > 1;

            var context = new AmazonSqsReceiveContext(message, redelivered, _context, _client, _receiveSettings, _client.ConnectionContext);
            try
            {
                await _dispatcher.Dispatch(context, context).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                context.LogTransportFaulted(exception);
            }
            finally
            {
                context.Dispose();
            }
        }

        async Task PollMessages(ChannelExecutor executor)
        {
            var maxReceiveCount = (_receiveSettings.PrefetchCount + 9) / 10;
            var receiveCount = 1;

            while (!IsStopping)
            {
                Task<int>[] receiveTasks = Enumerable.Repeat(0, receiveCount).Select(_ => ReceiveMessages(10, executor)).ToArray();

                int[] counts = await Task.WhenAll(receiveTasks).ConfigureAwait(false);

                var received = counts.Sum();

                if (received == receiveCount * 10) // ramp up receivers when busy
                    receiveCount = Math.Min(maxReceiveCount, receiveCount + (maxReceiveCount - receiveCount) / 2);
                else if (received / 10 < receiveCount - 1) // dial it back when not so busy
                    receiveCount = Math.Max(1, (received + 9) / 10);
            }
        }

        async Task<int> ReceiveMessages(int messageLimit, ChannelExecutor executor)
        {
            IList<Message> messages = await _client.ReceiveMessages(_receiveSettings.EntityName, messageLimit, _receiveSettings.WaitTimeSeconds, Stopping)
                .ConfigureAwait(false);

            await Task.WhenAll(messages.Select(message => executor.Push(() => HandleMessage(message), Stopping))).ConfigureAwait(false);

            return messages.Count;
        }

        Task HandleDeliveryComplete()
        {
            if (IsStopping)
            {
                LogContext.Debug?.Log("Consumer shutdown completed: {InputAddress}", _context.InputAddress);

                _deliveryComplete.TrySetResult(true);
            }

            return TaskUtil.Completed;
        }

        async Task ActiveAndActualAgentsCompleted(StopSupervisorContext context)
        {
            await Task.WhenAll(context.Agents.Select(x => Completed)).OrCanceled(context.CancellationToken).ConfigureAwait(false);

            if (_dispatcher.ActiveDispatchCount > 0)
            {
                try
                {
                    await _deliveryComplete.Task.OrCanceled(context.CancellationToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    LogContext.Warning?.Log("Stop canceled waiting for message consumers to complete: {InputAddress}", _context.InputAddress);
                }
            }
        }
    }
}
