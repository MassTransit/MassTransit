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
namespace MassTransit.Audit
{
    using System.Linq;


    public static class AuditMetadataFactories
    {
        public static readonly MessageAuditConsumeObserver.MetadataFactory DefaultConsumeContextMetadataFactory =
            c => new MessageAuditMetadata
            {
                ContextType = "Consume",
                ConversationId = c.ConversationId,
                CorrelationId = c.CorrelationId,
                InitiatorId = c.InitiatorId,
                MessageId = c.MessageId,
                RequestId = c.RequestId,
                DestinationAddress = c.DestinationAddress?.AbsoluteUri,
                SourceAddress = c.SourceAddress?.AbsoluteUri,
                FaultAddress = c.FaultAddress?.AbsoluteUri,
                ResponseAddress = c.ResponseAddress?.AbsoluteUri,
                Headers = c.Headers?.GetAll()?.ToDictionary(k => k.Key, v => v.Value.ToString())
            };

        public static readonly MessageAuditSendObserver.MetadataFactory DefaultSendContextMetadataFactory =
            c => new MessageAuditMetadata
            {
                ContextType = "Send",
                ConversationId = c.ConversationId,
                CorrelationId = c.CorrelationId,
                InitiatorId = c.InitiatorId,
                MessageId = c.MessageId,
                RequestId = c.RequestId,
                DestinationAddress = c.DestinationAddress?.AbsoluteUri,
                SourceAddress = c.SourceAddress?.AbsoluteUri,
                FaultAddress = c.FaultAddress?.AbsoluteUri,
                ResponseAddress = c.ResponseAddress?.AbsoluteUri,
                Headers = c.Headers?.GetAll()?.ToDictionary(k => k.Key, v => v.Value.ToString())
            };
    }
}