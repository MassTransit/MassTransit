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
    using System.Threading.Tasks;
    using GreenPipes;
    using Util;


    public class BatchConsumerFactory<TConsumer, TMessage> :
        IConsumerFactory<IConsumer<TMessage>>
        where TMessage : class
        where TConsumer : class, IConsumer<Batch<TMessage>>
    {
        readonly IConsumerFactory<TConsumer> _consumerFactory;
        readonly IPipe<ConsumerConsumeContext<TConsumer, Batch<TMessage>>> _consumerPipe;
        readonly int _messageLimit;
        readonly TimeSpan _timeLimit;
        readonly TaskScheduler _scheduler;
        BatchConsumer<TConsumer, TMessage> _currentConsumer;

        public BatchConsumerFactory(IConsumerFactory<TConsumer> consumerFactory, int messageLimit, TimeSpan timeLimit,
            IPipe<ConsumerConsumeContext<TConsumer, Batch<TMessage>>> consumerPipe)
        {
            _consumerFactory = consumerFactory;
            _messageLimit = messageLimit;
            _timeLimit = timeLimit;
            _consumerPipe = consumerPipe;

            _scheduler = new LimitedConcurrencyLevelTaskScheduler(1);
        }

        async Task IConsumerFactory<IConsumer<TMessage>>.Send<T>(ConsumeContext<T> context, IPipe<ConsumerConsumeContext<IConsumer<TMessage>, T>> next)
        {
            var messageContext = context as ConsumeContext<TMessage>;
            if (messageContext == null)
                throw new MessageException(typeof(T), $"Expected batch message type: {TypeMetadataCache<TMessage>.ShortName}");

            var consumer = await Task.Factory.StartNew(() => Add(messageContext), context.CancellationToken, TaskCreationOptions.None, _scheduler)
                .ConfigureAwait(false);

            await next.Send(context.PushConsumer(consumer)).ConfigureAwait(false);
        }

        BatchConsumer<TConsumer, TMessage> Add(ConsumeContext<TMessage> context)
        {
            if (_currentConsumer != null)
            {
                if (context.GetRetryAttempt() > 0)
                    _currentConsumer.ForceComplete();
            }

            if (_currentConsumer == null || _currentConsumer.IsCompleted)
                _currentConsumer = new BatchConsumer<TConsumer, TMessage>(_messageLimit, _timeLimit, _scheduler, _consumerFactory, _consumerPipe);

            _currentConsumer.Add(context);

            BatchConsumer<TConsumer, TMessage> consumer = _currentConsumer;

            return consumer;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateConsumerFactoryScope<IConsumer<TMessage>>("batch");

            _consumerFactory.Probe(scope);
        }
    }
}