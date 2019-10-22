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
namespace MassTransit.Tests.Audit
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GreenPipes.Caching;
    using MassTransit.Audit;
    using Metadata;
    using Util;


    public class InMemoryAuditStore :
        IMessageAuditStore,
        IEnumerable<Task<InMemoryAuditStore.AuditRecord>>
    {
        readonly ICache<AuditRecord> _audits;
        readonly IIndex<Guid, AuditRecord> _messageId;

        public InMemoryAuditStore()
        {
            var cacheSettings = new CacheSettings(10000, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(60));

            _audits = new GreenCache<AuditRecord>(cacheSettings);
            _messageId = _audits.AddIndex("messageId", x => x.Metadata.MessageId.Value);
        }

        Task IMessageAuditStore.StoreMessage<T>(T message, MessageAuditMetadata metadata)
        {
            _audits.Add(new AuditRecord<T>(message, metadata));

            return TaskUtil.Completed;
        }


        public interface AuditRecord
        {
            string MessageType { get; }
            MessageAuditMetadata Metadata { get; }
        }


        public class AuditRecord<T> :
            AuditRecord
            where T : class
        {
            public AuditRecord(T message, MessageAuditMetadata metadata)
            {
                Message = message;
                MessageType = TypeMetadataCache<T>.ShortName;
                Metadata = metadata;
            }

            public T Message { get; }
            public string MessageType { get; }
            public MessageAuditMetadata Metadata { get; }
        }


        public IEnumerator<Task<AuditRecord>> GetEnumerator()
        {
            return _audits.GetAll().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}