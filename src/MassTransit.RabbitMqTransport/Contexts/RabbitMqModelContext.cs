namespace MassTransit.RabbitMqTransport.Contexts
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using Integration;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using Util;


    public class RabbitMqModelContext :
        ScopePipeContext,
        ModelContext,
        IAsyncDisposable
    {
        readonly ConnectionContext _connectionContext;
        readonly IModel _model;
        readonly CancellationToken _cancellationToken;
        readonly ConcurrentDictionary<ulong, IPendingPublish> _published;
        readonly LimitedConcurrencyLevelTaskScheduler _taskScheduler;
        readonly Channel<Publish> _publishChannel;
        readonly Task _publishTask;
        readonly BatchSettings _batchSettings;
        ulong _publishTagMax;

        public RabbitMqModelContext(ConnectionContext connectionContext, IModel model, CancellationToken cancellationToken)
            : base(connectionContext)
        {
            _connectionContext = connectionContext;
            _model = model;
            _cancellationToken = cancellationToken;
            _batchSettings = connectionContext.BatchSettings;

            _published = new ConcurrentDictionary<ulong, IPendingPublish>();
            _taskScheduler = new LimitedConcurrencyLevelTaskScheduler(1);

            _model.ModelShutdown += OnModelShutdown;
            _model.BasicAcks += OnBasicAcks;
            _model.BasicNacks += OnBasicNacks;
            _model.BasicReturn += OnBasicReturn;

            if (_connectionContext.PublisherConfirmation)
            {
                _model.ConfirmSelect();

                if (_batchSettings.Enabled)
                {
                    var channelOptions = new BoundedChannelOptions(100)
                    {
                        AllowSynchronousContinuations = false,
                        FullMode = BoundedChannelFullMode.Wait,
                        SingleReader = true,
                        SingleWriter = false
                    };

                    _publishChannel = Channel.CreateBounded<Publish>(channelOptions);
                    _publishTask = Task.Run(WaitForBatch);
                }
            }
        }

        public async Task DisposeAsync(CancellationToken cancellationToken)
        {
            LogContext.Debug?.Log("Closing model: {ChannelNumber} {Host}", _model.ChannelNumber, _connectionContext.Description);

            try
            {
                if (_connectionContext.PublisherConfirmation && _model.IsOpen && _published.Count > 0)
                {
                    bool timedOut;
                    do
                    {
                        if (cancellationToken.IsCancellationRequested)
                            break;

                        _model.WaitForConfirms(_connectionContext.StopTimeout, out timedOut);
                        if (timedOut)
                            LogContext.Warning?.Log("Timeout waiting for pending confirms:  {ChannelNumber} {Host}", _model.ChannelNumber,
                                _connectionContext.Description);
                        else
                            LogContext.Debug?.Log("Pending confirms complete:  {ChannelNumber} {Host}", _model.ChannelNumber,
                                _connectionContext.Description);
                    }
                    while (timedOut);
                }

                if (!CancellationToken.IsCancellationRequested)
                    LogContext.Error?.Log("The CancellationToken should be requested at this point");

                if (_publishTask != null)
                    await _publishTask.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                LogContext.Error?.Log(ex, "Fault waiting for pending confirms:  {ChannelNumber} {Host}", _model.ChannelNumber,
                    _connectionContext.Description);
            }

            const string message = "ModelContext Disposed";

            _model.Cleanup(200, message);
        }

        CancellationToken PipeContext.CancellationToken => _cancellationToken;

        IModel ModelContext.Model => _model;

        ConnectionContext ModelContext.ConnectionContext => _connectionContext;

        async Task ModelContext.BasicPublishAsync(string exchange, string routingKey, bool mandatory, IBasicProperties basicProperties, byte[] body,
            bool awaitAck)
        {
            if (_connectionContext.PublisherConfirmation)
            {
                Task publishTask;
                if (mandatory || _batchSettings.Enabled == false)
                {
                    publishTask = await Task.Factory.StartNew(() => PublishAsync(exchange, routingKey, mandatory, basicProperties, body),
                        CancellationToken, TaskCreationOptions.None, _taskScheduler).ConfigureAwait(false);
                }
                else
                {
                    publishTask = await EnqueuePublish(exchange, routingKey, basicProperties, body).ConfigureAwait(false);
                }

                if (awaitAck)
                {
                    await publishTask.ConfigureAwait(false);
                    await Task.Yield();
                }
            }
            else
            {
                await Task.Factory.StartNew(() => PublishWithoutConfirmation(exchange, routingKey, mandatory, basicProperties, body),
                    CancellationToken, TaskCreationOptions.None, _taskScheduler).ConfigureAwait(false);
            }
        }

        Task ModelContext.ExchangeBind(string destination, string source, string routingKey, IDictionary<string, object> arguments)
        {
            return Task.Factory.StartNew(() => _model.ExchangeBind(destination, source, routingKey, arguments),
                CancellationToken, TaskCreationOptions.None, _taskScheduler);
        }

        Task ModelContext.ExchangeDeclare(string exchange, string type, bool durable, bool autoDelete, IDictionary<string, object> arguments)
        {
            return Task.Factory.StartNew(() => _model.ExchangeDeclare(exchange, type, durable, autoDelete, arguments),
                CancellationToken, TaskCreationOptions.None, _taskScheduler);
        }

        public Task ExchangeDeclarePassive(string exchange)
        {
            return Task.Factory.StartNew(() => _model.ExchangeDeclarePassive(exchange),
                CancellationToken, TaskCreationOptions.None, _taskScheduler);
        }

        Task ModelContext.QueueBind(string queue, string exchange, string routingKey, IDictionary<string, object> arguments)
        {
            return Task.Factory.StartNew(() => _model.QueueBind(queue, exchange, routingKey, arguments),
                CancellationToken, TaskCreationOptions.None, _taskScheduler);
        }

        Task<QueueDeclareOk> ModelContext.QueueDeclare(string queue, bool durable, bool exclusive, bool autoDelete, IDictionary<string, object> arguments)
        {
            return Task.Factory.StartNew(() => _model.QueueDeclare(queue, durable, exclusive, autoDelete, arguments),
                CancellationToken, TaskCreationOptions.None, _taskScheduler);
        }

        Task<QueueDeclareOk> ModelContext.QueueDeclarePassive(string queue)
        {
            return Task.Factory.StartNew(() => _model.QueueDeclarePassive(queue), CancellationToken, TaskCreationOptions.None, _taskScheduler);
        }

        Task<uint> ModelContext.QueuePurge(string queue)
        {
            return Task.Factory.StartNew(() => _model.QueuePurge(queue),
                CancellationToken, TaskCreationOptions.None, _taskScheduler);
        }

        Task ModelContext.BasicQos(uint prefetchSize, ushort prefetchCount, bool global)
        {
            return Task.Factory.StartNew(() => _model.BasicQos(prefetchSize, prefetchCount, global),
                CancellationToken, TaskCreationOptions.None, _taskScheduler);
        }

        void ModelContext.BasicAck(ulong deliveryTag, bool multiple)
        {
            _model.BasicAck(deliveryTag, multiple);
        }

        void ModelContext.BasicNack(ulong deliveryTag, bool multiple, bool requeue)
        {
            _model.BasicNack(deliveryTag, multiple, requeue);
        }

        Task<string> ModelContext.BasicConsume(string queue, bool noAck, bool exclusive, IDictionary<string, object> arguments, IBasicConsumer consumer)
        {
            return Task.Factory.StartNew(() => _model.BasicConsume(consumer, queue, noAck, "", false, exclusive, arguments),
                CancellationToken, TaskCreationOptions.None, _taskScheduler);
        }

        public Task BasicCancel(string consumerTag)
        {
            return Task.Factory.StartNew(() => _model.BasicCancel(consumerTag),
                CancellationToken, TaskCreationOptions.None, _taskScheduler);
        }

        void OnBasicReturn(object model, BasicReturnEventArgs args)
        {
            LogContext.Debug?.Log("BasicReturn: {ReplyCode}-{ReplyText} {MessageId}", args.ReplyCode, args.ReplyText, args.BasicProperties.MessageId);

            if (args.BasicProperties.Headers.TryGetValue("publishId", out var value))
            {
                var bytes = value as byte[];
                if (bytes == null)
                    return;

                if (!ulong.TryParse(Encoding.UTF8.GetString(bytes), out var id))
                    return;

                if (_published.TryRemove(id, out var published))
                    published.PublishReturned(args.ReplyCode, args.ReplyText);
            }
        }

        Task PublishAsync(string exchange, string routingKey, bool mandatory, IBasicProperties basicProperties, byte[] body)
        {
            var publishTag = _model.NextPublishSeqNo;
            _publishTagMax = Math.Max(_publishTagMax, publishTag);
            var pendingPublish = new PendingPublish(_connectionContext, exchange, publishTag);
            try
            {
                _published.AddOrUpdate(publishTag, key => pendingPublish, (key, existing) =>
                {
                    existing.PublishNotConfirmed($"Duplicate key: {key}");

                    return pendingPublish;
                });

                basicProperties.Headers["publishId"] = publishTag.ToString("F0");

                _model.BasicPublish(exchange, routingKey, mandatory, basicProperties, body);
            }
            catch
            {
                _published.TryRemove(publishTag, out _);

                throw;
            }

            return pendingPublish.Task;
        }

        async Task<Task> EnqueuePublish(string exchange, string routingKey, IBasicProperties basicProperties, byte[] body)
        {
            var publish = new Publish(exchange, routingKey, basicProperties, body);

            await _publishChannel.Writer.WriteAsync(publish, CancellationToken).ConfigureAwait(false);

            return publish.Task;
        }

        async Task WaitForBatch()
        {
            while (CancellationToken.IsCancellationRequested == false)
            {
                try
                {
                    while (await _publishChannel.Reader.WaitToReadAsync(CancellationToken).ConfigureAwait(false))
                    {
                        await ReadBatch().ConfigureAwait(false);
                    }
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception exception)
                {
                    LogContext.Error?.Log(exception, "PublishBatch Faulted");
                }
            }

            try
            {
                while (_publishChannel.Reader.TryRead(out var publish))
                    publish.PublishNotConfirmed("Model Context Canceled");
            }
            catch (Exception exception)
            {
                LogContext.Error?.Log(exception, "Publish Channel Cleanup Faulted");
            }
        }

        async Task ReadBatch()
        {
            var batchToken = new CancellationTokenSource(_batchSettings.Timeout);
            List<Publish> batch = new List<Publish>();
            try
            {
                try
                {
                    for (int i = 0,
                        batchLength = 0;
                        i < _batchSettings.MessageLimit && batchLength < _batchSettings.SizeLimit;
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

                await Task.Factory.StartNew(() => PublishBatch(batch), CancellationToken, TaskCreationOptions.None, _taskScheduler).ConfigureAwait(false);
            }
            catch (OperationCanceledException exception) when (exception.CancellationToken == batchToken.Token)
            {
            }
            catch (OperationCanceledException exception) when (CancellationToken.IsCancellationRequested)
            {
                // make sure batch items are cancelled, since the PublishBatch method may not have executed
                for (int i = 0; i < batch.Count; i++)
                    batch[i].PublishNotConfirmed(exception.Message);
            }
            finally
            {
                batchToken.Dispose();
            }
        }

        async Task PublishBatch(IList<Publish> batch)
        {
            var publishTag = _model.NextPublishSeqNo;
            try
            {
                if (batch.Count == 1)
                {
                    var published = batch[0];
                    _published.AddOrUpdate(publishTag, key => published, (key, existing) =>
                    {
                        existing.PublishNotConfirmed($"Duplicate key: {key}");

                        return published;
                    });

                    published.PublishOne(_model, publishTag);
                    _publishTagMax = Math.Max(_publishTagMax, publishTag);
                }
                else
                {
                    var publishBatch = _model.CreateBasicPublishBatch();

                    for (int i = 0; i < batch.Count; i++, publishTag++)
                    {
                        batch[i].Append(publishBatch, publishTag);

                        var published = batch[i];
                        _published.AddOrUpdate(publishTag, key => published, (key, existing) =>
                        {
                            existing.PublishNotConfirmed($"Duplicate key: {key}");

                            return published;
                        });
                    }

                    publishBatch.Publish();

                    _publishTagMax = Math.Max(_publishTagMax, publishTag);

                    var nextPublishTag = _model.NextPublishSeqNo;
                    if (nextPublishTag <= publishTag)
                    {
                        LogContext.Warning?.Log("Batch Publish SeqNo Mismatch: {Expected} != {Actual}", publishTag + 1, nextPublishTag);
                    }
                }
            }
            catch (Exception exception)
            {
                for (int i = 0; i < batch.Count; i++)
                {
                    _published.TryRemove(batch[i].Tag, out _);

                    batch[i].PublishNotConfirmed(exception.Message);
                }
            }
        }

        void PublishWithoutConfirmation(string exchange, string routingKey, bool mandatory, IBasicProperties basicProperties, byte[] body)
        {
            var publishTag = _model.NextPublishSeqNo;
            _publishTagMax = Math.Max(_publishTagMax, publishTag);

            _model.BasicPublish(exchange, routingKey, mandatory, basicProperties, body);
        }

        void OnModelShutdown(object model, ShutdownEventArgs reason)
        {
            _model.ModelShutdown -= OnModelShutdown;
            _model.BasicAcks -= OnBasicAcks;
            _model.BasicNacks -= OnBasicNacks;
            _model.BasicReturn -= OnBasicReturn;

            FaultPendingPublishes(reason.ReplyText);
        }

        void FaultPendingPublishes(string reason)
        {
            try
            {
                foreach (var key in _published.Keys)
                {
                    if (_published.TryRemove(key, out var pending))
                        pending.PublishNotConfirmed(reason);
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        void OnBasicNacks(object model, BasicNackEventArgs args)
        {
            if (args.Multiple)
            {
                ulong[] ids = _published.Keys.Where(x => x <= args.DeliveryTag).ToArray();
                foreach (var id in ids)
                {
                    if (_published.TryRemove(id, out var value))
                        value.Nack();
                }

                Console.WriteLine($"Negative Acknowledgement: {string.Join(",", ids.Select(x => x.ToString()))}, Remaining: {_published.Count}");
            }
            else
            {
                if (_published.TryRemove(args.DeliveryTag, out var value))
                    value.Nack();

                Console.WriteLine($"Negative Acknowledgement: {args.DeliveryTag}, Remaining: {_published.Count}");
            }
        }

        void OnBasicAcks(object model, BasicAckEventArgs args)
        {
            if (args.Multiple)
            {
                ulong[] ids = _published.Keys.Where(x => x <= args.DeliveryTag).ToArray();
                foreach (var id in ids)
                {
                    if (_published.TryRemove(id, out var value))
                        value.Ack();
                }

                Console.WriteLine($"Acknowledged: {string.Join(",", ids.Select(x => x.ToString()))}, Remaining: {_published.Count}");
            }
            else
            {
                if (_published.TryRemove(args.DeliveryTag, out var value))
                    value.Ack();

                Console.WriteLine($"Acknowledged: {args.DeliveryTag}, Remaining: {_published.Count}");
            }
        }
    }
}
