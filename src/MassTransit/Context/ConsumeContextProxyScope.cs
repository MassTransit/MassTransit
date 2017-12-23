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
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Payloads;


    public class ConsumeContextProxyScope :
        ConsumeContextProxy
    {
        public ConsumeContextProxyScope(ConsumeContext context)
            : base(context, new PayloadCacheScope(context))
        {
        }
    }


    public abstract class ConsumeContextProxyScope<TMessage> :
        ConsumeContextProxy<TMessage>
        where TMessage : class
    {
        readonly Lazy<IPublishEndpoint> _publishEndpoint;

        protected ConsumeContextProxyScope(ConsumeContext<TMessage> context)
            : base(context, new PayloadCacheScope(context))
        {
            _publishEndpoint = new Lazy<IPublishEndpoint>(() => new ConsumeContextScopePublishEndpoint(this, context));
        }

        public override Task Publish<T>(T message, CancellationToken cancellationToken)
        {
            var task = _publishEndpoint.Value.Publish(message, cancellationToken);
            ReceiveContext.AddPendingTask(task);
            return task;
        }

        public override Task Publish<T>(T message, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
        {
            var task = _publishEndpoint.Value.Publish(message, publishPipe, cancellationToken);
            ReceiveContext.AddPendingTask(task);
            return task;
        }

        public override Task Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            var task = _publishEndpoint.Value.Publish(message, publishPipe, cancellationToken);
            ReceiveContext.AddPendingTask(task);
            return task;
        }

        public override Task Publish(object message, CancellationToken cancellationToken)
        {
            var task = _publishEndpoint.Value.Publish(message, cancellationToken);
            ReceiveContext.AddPendingTask(task);
            return task;
        }

        public override Task Publish(object message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            var task = _publishEndpoint.Value.Publish(message, publishPipe, cancellationToken);
            ReceiveContext.AddPendingTask(task);
            return task;
        }

        public override Task Publish(object message, Type messageType, CancellationToken cancellationToken)
        {
            var task = _publishEndpoint.Value.Publish(message, messageType, cancellationToken);
            ReceiveContext.AddPendingTask(task);
            return task;
        }

        public override Task Publish(object message, Type messageType, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            var task = _publishEndpoint.Value.Publish(message, messageType, publishPipe, cancellationToken);
            ReceiveContext.AddPendingTask(task);
            return task;
        }

        public override Task Publish<T>(object values, CancellationToken cancellationToken)
        {
            var task = _publishEndpoint.Value.Publish<T>(values, cancellationToken);
            ReceiveContext.AddPendingTask(task);
            return task;
        }

        public override Task Publish<T>(object values, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
        {
            var task = _publishEndpoint.Value.Publish(values, publishPipe, cancellationToken);
            ReceiveContext.AddPendingTask(task);
            return task;
        }

        public override Task Publish<T>(object values, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            var task = _publishEndpoint.Value.Publish<T>(values, publishPipe, cancellationToken);
            ReceiveContext.AddPendingTask(task);
            return task;
        }
    }
}