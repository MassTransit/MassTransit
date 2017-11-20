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
namespace MassTransit.Audit.MetadataFactories
{
    using System.Linq;


    public class DefaultSendMetadataFactory :
        ISendMetadataFactory
    {
        MessageAuditMetadata ISendMetadataFactory.CreateAuditMetadata<T>(SendContext<T> context)
        {
            return CreateMetadata(context, "Send");
        }

        MessageAuditMetadata ISendMetadataFactory.CreateAuditMetadata<T>(PublishContext<T> context)
        {
            return CreateMetadata(context, "Publish");
        }

        static MessageAuditMetadata CreateMetadata(SendContext context, string contextType)
        {
            return new MessageAuditMetadata
            {
                ContextType = contextType,
                ConversationId = context.ConversationId,
                CorrelationId = context.CorrelationId,
                InitiatorId = context.InitiatorId,
                MessageId = context.MessageId,
                RequestId = context.RequestId,
                DestinationAddress = context.DestinationAddress?.AbsoluteUri,
                SourceAddress = context.SourceAddress?.AbsoluteUri,
                FaultAddress = context.FaultAddress?.AbsoluteUri,
                ResponseAddress = context.ResponseAddress?.AbsoluteUri,
                InputAddress = "",
                Headers = context.Headers?.GetAll()?.ToDictionary(k => k.Key, v => v.Value.ToString())
            };
        }
    }
}