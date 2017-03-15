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
namespace MassTransit.Audit.Tests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GreenPipes.Util;


    public class InMemoryAuditStore : MessageAuditStore
    {
        internal List<AuditRecord> Audit = new List<AuditRecord>();

        public Task StoreMessage(object message, string messageType, MessageAuditMetadata metadata)
        {
            Audit.Add(new AuditRecord(message, messageType, metadata));
            return TaskUtil.Completed;
        }


        internal class AuditRecord
        {
            public AuditRecord(object message, string messageType, MessageAuditMetadata metadata)
            {
                Message = message;
                MessageType = messageType;
                Metadata = metadata;
            }

            public object Message { get; }
            public string MessageType { get; }
            public MessageAuditMetadata Metadata { get; }
        }
    }

}