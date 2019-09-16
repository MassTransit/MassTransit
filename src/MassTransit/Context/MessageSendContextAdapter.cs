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
namespace MassTransit.Context
{
    using System;
    using Metadata;
    using Util;


    public class MessageSendContextAdapter :
        MessageContext
    {
        readonly SendContext _context;

        public MessageSendContextAdapter(SendContext context)
        {
            _context = context;
        }

        Guid? MessageContext.MessageId => _context.MessageId;

        Guid? MessageContext.RequestId => _context.RequestId;

        Guid? MessageContext.CorrelationId => _context.CorrelationId;

        Guid? MessageContext.ConversationId => _context.ConversationId;

        Guid? MessageContext.InitiatorId => _context.InitiatorId;

        DateTime? MessageContext.ExpirationTime => _context.TimeToLive.HasValue ? DateTime.UtcNow + _context.TimeToLive : default(DateTime?);

        Uri MessageContext.SourceAddress => _context.SourceAddress;

        Uri MessageContext.DestinationAddress => _context.DestinationAddress;

        Uri MessageContext.ResponseAddress => _context.ResponseAddress;

        Uri MessageContext.FaultAddress => _context.FaultAddress;

        DateTime? MessageContext.SentTime => default(DateTime?);

        Headers MessageContext.Headers => _context.Headers;

        HostInfo MessageContext.Host => HostMetadataCache.Host;
    }
}