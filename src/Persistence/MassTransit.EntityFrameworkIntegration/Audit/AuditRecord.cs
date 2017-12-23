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
namespace MassTransit.EntityFrameworkIntegration.Audit
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using MassTransit.Audit;
    using Newtonsoft.Json;

    public class AuditRecord
    {
        public int AuditRecordId { get; set; }
        public Guid? MessageId { get; set; }
        public Guid? ConversationId { get; set; }
        public Guid? CorrelationId { get; set; }
        public Guid? InitiatorId { get; set; }
        public Guid? RequestId { get; set; }
        public string SourceAddress { get; set; }
        public string DestinationAddress { get; set; }
        public string ResponseAddress { get; set; }
        public string FaultAddress { get; set; }
        public string InputAddress { get; set; }
        public string ContextType { get; set; }
        public string MessageType { get; set; }

        internal string _custom { get; set; }

        [NotMapped]
        public Dictionary<string, string> Custom
        {
            get => string.IsNullOrEmpty(_custom)
                       ? new Dictionary<string, string>()
                       : JsonConvert.DeserializeObject<Dictionary<string, string>>(_custom);
            set => _custom = JsonConvert.SerializeObject(value);
        }

        internal string _headers { get; set; }

        [NotMapped]
        public Dictionary<string, string> Headers
        {
            get => string.IsNullOrEmpty(_headers)
                       ? new Dictionary<string, string>()
                       : JsonConvert.DeserializeObject<Dictionary<string, string>>(_headers);
            set => _headers = JsonConvert.SerializeObject(value);
        }

        internal string _message { get; set; }

        [NotMapped]
        public object Message
        {
            get => string.IsNullOrEmpty(_message)
                       ? null
                       : JsonConvert.DeserializeObject(_message);
            set => _message = JsonConvert.SerializeObject(value);
        }

        internal static AuditRecord Create<T>(T message, string messageType, MessageAuditMetadata metadata)
            where T : class
        {
            return new AuditRecord
            {
                ContextType = metadata.ContextType,
                MessageId = metadata.MessageId,
                ConversationId = metadata.ConversationId,
                CorrelationId = metadata.CorrelationId,
                InitiatorId = metadata.InitiatorId,
                RequestId = metadata.RequestId,
                SourceAddress = metadata.SourceAddress,
                DestinationAddress = metadata.DestinationAddress,
                ResponseAddress = metadata.ResponseAddress,
                FaultAddress = metadata.FaultAddress,
                Headers = metadata.Headers,
                Custom = metadata.Custom,
                Message = message,
                MessageType = messageType
            };
        }
    }
}