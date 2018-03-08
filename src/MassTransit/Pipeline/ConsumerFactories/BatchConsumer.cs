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
namespace MassTransit.Pipeline.ConsumerFactories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;


    public class BatchConsumer<TConsumer, TMessage> :
        IConsumer<TMessage>
        where TMessage : class
        where TConsumer : class
    {
        readonly TaskCompletionSource<DateTime> _completed;
        readonly IConsumerFactory<TConsumer> _consumerFactory;
        readonly IPipe<ConsumerConsumeContext<TConsumer, Batch<TMessage>>> _consumerPipe;
        readonly DateTime _firstMessage;
        readonly int _messageLimit;
        readonly TaskScheduler _taskScheduler;
        readonly SortedDictionary<Guid, ConsumeContext<TMessage>> _messages;
        readonly Timer _timer;
        bool _isCompleted;
        DateTime _lastMessage;

        public BatchConsumer(int messageLimit, TimeSpan timeLimit, TaskScheduler taskScheduler, IConsumerFactory<TConsumer> consumerFactory,
            IPipe<ConsumerConsumeContext<TConsumer, Batch<TMessage>>> consumerPipe)
        {
            _messageLimit = messageLimit;
            _taskScheduler = taskScheduler;
            _consumerFactory = consumerFactory;
            _consumerPipe = consumerPipe;
            _messages = new SortedDictionary<Guid, ConsumeContext<TMessage>>();
            _completed = new TaskCompletionSource<DateTime>();
            _firstMessage = DateTime.UtcNow;

            _timer = new Timer(TimeLimitExpired, null, timeLimit, TimeSpan.Zero);
        }

        public bool IsCompleted => _isCompleted;

        async Task IConsumer<TMessage>.Consume(ConsumeContext<TMessage> context)
        {
            try
            {
                await _completed.Task.ConfigureAwait(false);
            }
            catch
            {
                // if this message was marked as successfully delivered, do not fault it
                if (context.ReceiveContext.IsDelivered)
                    return;

                // again, if it's already faulted, we don't want to fault it again
                if (context.ReceiveContext.IsFaulted)
                    return;

                throw;
            }
        }

        void TimeLimitExpired(object state)
        {
            Task.Factory.StartNew(() =>
            {
                _isCompleted = true;

                if (_messages.Count > 0)
                {
                    ConsumeContext<TMessage>[] messages = _messages.Values.ToArray();
                    Deliver(messages[0], messages, BatchCompletionMode.Time);
                }
            }, CancellationToken.None, TaskCreationOptions.None, _taskScheduler);
        }

        public void Add(ConsumeContext<TMessage> context)
        {
            var messageId = context.MessageId ?? NewId.NextGuid();
            _messages.Add(messageId, context);

            _lastMessage = DateTime.UtcNow;

            if (IsReadyToDeliver(context))
            {
                _isCompleted = true;

                Deliver(context, _messages.Values.ToArray(), BatchCompletionMode.Size);
            }
        }

        bool IsReadyToDeliver(ConsumeContext<TMessage> context)
        {
            if (context.GetRetryAttempt() > 0)
                return true;

            return _messages.Count == _messageLimit;
        }

        public void ForceComplete()
        {
            _isCompleted = true;

            ConsumeContext<TMessage>[] consumeContexts = _messages.Values.ToArray();
            if (consumeContexts.Length == 0)
                return;

            Deliver(consumeContexts.Last(), consumeContexts, BatchCompletionMode.Forced);
        }

        async void Deliver(ConsumeContext context, ConsumeContext<TMessage>[] messages, BatchCompletionMode batchCompletionMode)
        {
            _timer.Dispose();

            Batch<TMessage> batch = new Batch(_firstMessage, _lastMessage, batchCompletionMode, messages);

            try
            {
                var proxy = new MessageConsumeContext<Batch<TMessage>>(context, batch);

                await _consumerFactory.Send(proxy, _consumerPipe).ConfigureAwait(false);

                _completed.TrySetResult(DateTime.UtcNow);
            }
            catch (OperationCanceledException exception) when (exception.CancellationToken == context.CancellationToken)
            {
                _completed.TrySetCanceled();
            }
            catch (Exception exception)
            {
                _completed.TrySetException(exception);
            }
        }


        class Batch :
            Batch<TMessage>
        {
            readonly ConsumeContext<TMessage>[] _messages;

            public Batch(DateTime firstMessageReceived, DateTime lastMessageReceived, BatchCompletionMode mode, ConsumeContext<TMessage>[] messages)
            {
                FirstMessageReceived = firstMessageReceived;
                LastMessageReceived = lastMessageReceived;
                Mode = mode;
                _messages = messages;
            }

            public BatchCompletionMode Mode { get; }
            public DateTime FirstMessageReceived { get; }
            public DateTime LastMessageReceived { get; }

            public ConsumeContext<TMessage> this[int index] => _messages[index];

            public int Length => _messages.Length;
        }
    }
}