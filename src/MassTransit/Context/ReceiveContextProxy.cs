// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.IO;
    using System.Net.Mime;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Topology;


    public abstract class ReceiveContextProxy :
        ReceiveContext
    {
        readonly ReceiveContext _context;

        protected ReceiveContextProxy(ReceiveContext context)
        {
            _context = context;
        }

        public CancellationToken CancellationToken => _context.CancellationToken;

        public virtual bool HasPayloadType(Type contextType)
        {
            return _context.HasPayloadType(contextType);
        }

        public virtual bool TryGetPayload<TPayload>(out TPayload payload)
            where TPayload : class
        {
            return _context.TryGetPayload(out payload);
        }

        public virtual TPayload GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory)
            where TPayload : class
        {
            return _context.GetOrAddPayload(payloadFactory);
        }

        T PipeContext.AddOrUpdatePayload<T>(PayloadFactory<T> addFactory, UpdatePayloadFactory<T> updateFactory)
        {
            return _context.AddOrUpdatePayload(addFactory, updateFactory);
        }

        public Stream GetBodyStream()
        {
            return _context.GetBodyStream();
        }

        byte[] ReceiveContext.GetBody()
        {
            return _context.GetBody();
        }

        public TimeSpan ElapsedTime => _context.ElapsedTime;
        public Uri InputAddress => _context.InputAddress;
        public ContentType ContentType => _context.ContentType;
        public bool Redelivered => _context.Redelivered;
        public Headers TransportHeaders => _context.TransportHeaders;
        public Task ReceiveCompleted => _context.ReceiveCompleted;
        public bool IsDelivered => _context.IsDelivered;
        public bool IsFaulted => _context.IsFaulted;

        public virtual Task NotifyConsumed<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
            where T : class
        {
            return _context.NotifyConsumed(context, duration, consumerType);
        }

        public virtual Task NotifyFaulted<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
            where T : class
        {
            return _context.NotifyFaulted(context, duration, consumerType, exception);
        }

        public Task NotifyFaulted(Exception exception)
        {
            return _context.NotifyFaulted(exception);
        }

        public virtual void AddReceiveTask(Task task)
        {
            _context.AddReceiveTask(task);
        }

        public virtual ISendEndpointProvider SendEndpointProvider => _context.SendEndpointProvider;
        public virtual IPublishEndpointProvider PublishEndpointProvider => _context.PublishEndpointProvider;
        IPublishTopology ReceiveContext.PublishTopology => _context.PublishTopology;
    }
}
