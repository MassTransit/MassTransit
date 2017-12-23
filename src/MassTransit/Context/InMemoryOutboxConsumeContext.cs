// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Context
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Pipeline.Filters.Outbox;
    using Util;


    public class InMemoryOutboxConsumeContext :
        ConsumeContextProxy,
        OutboxContext
    {
        readonly TaskCompletionSource<InMemoryOutboxConsumeContext> _clearToSend;
        readonly List<Func<Task>> _pendingActions;

        public InMemoryOutboxConsumeContext(ConsumeContext context)
            : base(context)
        {
            _pendingActions = new List<Func<Task>>();
            _clearToSend = new TaskCompletionSource<InMemoryOutboxConsumeContext>();
        }

        public Task ClearToSend => _clearToSend.Task;

        public void Add(Func<Task> method)
        {
            lock (_pendingActions)
                _pendingActions.Add(method);
        }

        public override async Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            var endpoint = await base.GetSendEndpoint(address).ConfigureAwait(false);

            return new OutboxSendEndpoint(this, endpoint);
        }

        public override Task Publish<T>(T message, CancellationToken cancellationToken)
        {
            Add(() => base.Publish(message, cancellationToken));

            return TaskUtil.Completed;
        }

        public override Task Publish<T>(T message, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
        {
            Add(() => base.Publish(message, publishPipe, cancellationToken));

            return TaskUtil.Completed;
        }

        public override Task Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            Add(() => base.Publish(message, publishPipe, cancellationToken));

            return TaskUtil.Completed;
        }

        public override Task Publish(object message, CancellationToken cancellationToken)
        {
            Add(() => base.Publish(message, cancellationToken));

            return TaskUtil.Completed;
        }

        public override Task Publish(object message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            Add(() => base.Publish(message, publishPipe, cancellationToken));

            return TaskUtil.Completed;
        }

        public override Task Publish(object message, Type messageType, CancellationToken cancellationToken)
        {
            Add(() => base.Publish(message, messageType, cancellationToken));

            return TaskUtil.Completed;
        }

        public override Task Publish(object message, Type messageType, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            Add(() => base.Publish(message, messageType, publishPipe, cancellationToken));

            return TaskUtil.Completed;
        }

        public override Task Publish<T>(object values, CancellationToken cancellationToken)
        {
            Add(() => base.Publish<T>(values, cancellationToken));

            return TaskUtil.Completed;
        }

        public override Task Publish<T>(object values, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
        {
            Add(() => base.Publish(values, publishPipe, cancellationToken));

            return TaskUtil.Completed;
        }

        public override Task Publish<T>(object values, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            Add(() => base.Publish<T>(values, publishPipe, cancellationToken));

            return TaskUtil.Completed;
        }

        public override Task RespondAsync<T>(T message)
        {
            Add(() => base.RespondAsync(message));

            return TaskUtil.Completed;
        }

        public override Task RespondAsync(object message, Type messageType, IPipe<SendContext> sendPipe)
        {
            Add(() => base.RespondAsync(message, messageType, sendPipe));

            return TaskUtil.Completed;
        }

        public override Task RespondAsync<T>(object values)
        {
            Add(() => base.RespondAsync<T>(values));

            return TaskUtil.Completed;
        }

        public override Task RespondAsync<T>(object values, IPipe<SendContext<T>> sendPipe)
        {
            Add(() => base.RespondAsync(values, sendPipe));

            return TaskUtil.Completed;
        }

        public override Task RespondAsync<T>(object values, IPipe<SendContext> sendPipe)
        {
            Add(() => base.RespondAsync<T>(values, sendPipe));

            return TaskUtil.Completed;
        }

        public override Task RespondAsync<T>(T message, IPipe<SendContext<T>> sendPipe)
        {
            Add(() => base.RespondAsync(message, sendPipe));

            return TaskUtil.Completed;
        }

        public override Task RespondAsync<T>(T message, IPipe<SendContext> sendPipe)
        {
            Add(() => base.RespondAsync(message, sendPipe));

            return TaskUtil.Completed;
        }

        public override Task RespondAsync(object message)
        {
            Add(() => base.RespondAsync(message));

            return TaskUtil.Completed;
        }

        public override Task RespondAsync(object message, Type messageType)
        {
            Add(() => base.RespondAsync(message, messageType));

            return TaskUtil.Completed;
        }

        public override Task RespondAsync(object message, IPipe<SendContext> sendPipe)
        {
            Add(() => base.RespondAsync(message, sendPipe));

            return TaskUtil.Completed;
        }

        public override void Respond<T>(T message)
        {
            Add(() =>
            {
                base.Respond(message);

                return TaskUtil.Completed;
            });
        }

        public async Task ExecutePendingActions()
        {
            _clearToSend.TrySetResult(this);

            Func<Task>[] pendingActions;
            lock (_pendingActions)
                pendingActions = _pendingActions.ToArray();

            foreach (var action in pendingActions)
            {
                var task = action();
                if (task != null)
                    await task.ConfigureAwait(false);
            }
        }

        public Task DiscardPendingActions()
        {
            return TaskUtil.Completed;
        }
    }
}