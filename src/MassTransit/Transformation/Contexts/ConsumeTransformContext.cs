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
namespace MassTransit.Transformation.Contexts
{
    using System;
    using GreenPipes;
    using GreenPipes.Payloads;


    /// <summary>
    /// Sits in front of the consume context and allows the inbound message to be 
    /// transformed.
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public class ConsumeTransformContext<TMessage> :
        BasePipeContext,
        TransformContext<TMessage>
        where TMessage : class
    {
        readonly ConsumeContext<TMessage> _context;

        public ConsumeTransformContext(ConsumeContext<TMessage> context)
            : base(new PayloadCacheScope(context))
        {
            _context = context;
        }

        Guid? TransformContext.MessageId => _context.MessageId;
        Guid? TransformContext.RequestId => _context.RequestId;
        Guid? TransformContext.CorrelationId => _context.CorrelationId;
        Guid? TransformContext.ConversationId => _context.ConversationId;
        Guid? TransformContext.InitiatorId => _context.InitiatorId;
        Uri TransformContext.SourceAddress => _context.SourceAddress;
        Uri TransformContext.DestinationAddress => _context.DestinationAddress;
        Headers TransformContext.Headers => _context.Headers;
        HostInfo TransformContext.Host => _context.Host;

        TMessage TransformContext<TMessage>.Input => _context.Message;

        bool TransformContext<TMessage>.HasInput => true;
    }
}