namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using Configuration;
    using RabbitMQ.Client;
    using Util;


    public class BatchPublisher :
        IPublisher
    {
        readonly PendingConfirmationCollection _confirmations;
        readonly ChannelExecutor _executor;
        readonly IPublisher _immediatePublisher;
        readonly IModel _model;
        readonly Channel<BatchPublish> _publishChannel;
        readonly Task _publishTask;
        readonly BatchSettings _settings;

        public BatchPublisher(ChannelExecutor executor, IModel model, BatchSettings settings, PendingConfirmationCollection confirmations)
        {
            _executor = executor;
            _model = model;
            _settings = settings;
            _confirmations = confirmations;

            _immediatePublisher = new ImmediatePublisher(executor, model, confirmations);

            var channelOptions = new BoundedChannelOptions(settings.MessageLimit)
            {
                AllowSynchronousContinuations = false,
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true,
                SingleWriter = false
            };

            _publishChannel = Channel.CreateBounded<BatchPublish>(channelOptions);
            _publishTask = Task.Run(WaitForBatch);
        }

        public Task Publish(string exchange, string routingKey, bool mandatory, IBasicProperties basicProperties, byte[] body, bool awaitAck)
        {
            if (mandatory)
                return _immediatePublisher.Publish(exchange, routingKey, true, basicProperties, body, awaitAck);

            var publish = new BatchPublish(exchange, routingKey, basicProperties, body, awaitAck);

            async Task PublishAsync()
            {
                await _publishChannel.Writer.WriteAsync(publish).ConfigureAwait(false);

                await publish.Confirmed.ConfigureAwait(false);
            }

            return PublishAsync();
        }

        public async ValueTask DisposeAsync()
        {
            _publishChannel.Writer.Complete();

            await _publishTask.ConfigureAwait(false);
        }

        async Task WaitForBatch()
        {
            try
            {
                while (await _publishChannel.Reader.WaitToReadAsync().ConfigureAwait(false))
                    await ReadBatch().ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
            catch (ChannelClosedException)
            {
            }
            catch (Exception exception)
            {
                LogContext.Error?.Log(exception, "WaitForBatch Faulted");
            }
        }

        async Task ReadBatch()
        {
            var batchToken = new CancellationTokenSource(_settings.Timeout);
            var batch = new List<BatchPublish>();
            try
            {
                try
                {
                    for (int i = 0,
                        batchLength = 0;
                        i < _settings.MessageLimit && batchLength < _settings.SizeLimit;
                        i++)
                    {
                        var publish = await _publishChannel.Reader.ReadAsync(batchToken.Token).ConfigureAwait(false);

                        batch.Add(publish);
                        batchLength += publish.Length;

                        if (await _publishChannel.Reader.WaitToReadAsync(batchToken.Token).ConfigureAwait(false) == false)
                            break;
                    }
                }
                catch (OperationCanceledException exception) when (exception.CancellationToken == batchToken.Token && batch.Count > 0)
                {
                }

                await _executor.Run(() => PublishBatch(batch)).ConfigureAwait(false);
            }
            catch (OperationCanceledException exception) when (exception.CancellationToken == batchToken.Token)
            {
            }
            catch (Exception exception)
            {
                for (var i = 0; i < batch.Count; i++)
                    batch[i].NotConfirmed(exception);
            }
            finally
            {
                batchToken.Dispose();
            }
        }

        async Task PublishBatch(IList<BatchPublish> batch)
        {
            try
            {
                var publishTag = _model.NextPublishSeqNo;
                if (batch.Count == 1)
                {
                    var publish = batch[0];

                    _confirmations?.Add(publishTag, publish);

                    publish.BasicPublish(_model, publishTag);
                }
                else
                {
                    var publishBatch = _model.CreateBasicPublishBatch();

                    for (var i = 0; i < batch.Count; i++)
                    {
                        var publish = batch[i];

                        publish.Append(publishBatch, publishTag);

                        _confirmations?.Add(publishTag++, publish);
                    }

                    publishBatch.Publish();

                    for (var i = 0; i < batch.Count; i++)
                        batch[i].Published();
                }
            }
            catch (Exception exception)
            {
                for (var i = 0; i < batch.Count; i++)
                {
                    var publish = batch[i];

                    _confirmations?.Faulted(publish);

                    publish.NotConfirmed(exception);
                }
            }
        }
    }
}
