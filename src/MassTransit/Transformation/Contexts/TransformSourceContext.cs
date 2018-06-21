// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Transformation.Contexts
{
    using System;
    using System.Threading;
    using GreenPipes;


    public class TransformSourceContext<TProperty, TInput> :
        SourceContext<TProperty, TInput>
    {
        readonly TransformContext<TInput> _context;

        public TransformSourceContext(TransformContext<TInput> context)
        {
            _context = context;
        }

        public TransformSourceContext(TransformContext<TInput> context, TProperty value)
        {
            _context = context;
            Value = value;
            HasValue = true;
        }

        CancellationToken TransformContext.CancellationToken => _context.CancellationToken;
        Guid? TransformContext.MessageId => _context.MessageId;
        Guid? TransformContext.RequestId => _context.RequestId;
        Guid? TransformContext.CorrelationId => _context.CorrelationId;
        Guid? TransformContext.ConversationId => _context.ConversationId;
        Guid? TransformContext.InitiatorId => _context.InitiatorId;
        Uri TransformContext.SourceAddress => _context.SourceAddress;
        Uri TransformContext.DestinationAddress => _context.DestinationAddress;
        Headers TransformContext.Headers => _context.Headers;
        HostInfo TransformContext.Host => _context.Host;

        bool TransformContext.HasPayloadType(Type contextType)
        {
            return _context.HasPayloadType(contextType);
        }

        bool TransformContext.TryGetPayload<TPayload>(out TPayload payload)
        {
            return _context.TryGetPayload(out payload);
        }

        TPayload TransformContext.GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory)
        {
            return _context.GetOrAddPayload(payloadFactory);
        }

        public TInput Input => _context.Input;

        public bool HasInput => _context.HasInput;

        public bool HasValue { get; }

        public TProperty Value { get; }
    }
}