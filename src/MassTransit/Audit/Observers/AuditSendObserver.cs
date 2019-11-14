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
namespace MassTransit.Audit.Observers
{
    using System;
    using System.Threading.Tasks;
    using Util;
    using Util.Scanning;


    public class AuditSendObserver :
        ISendObserver
    {
        readonly ISendMetadataFactory _metadataFactory;
        readonly CompositeFilter<SendContext> _filter;
        readonly IMessageAuditStore _store;

        public AuditSendObserver(IMessageAuditStore store, ISendMetadataFactory metadataFactory, CompositeFilter<SendContext> filter)
        {
            _store = store;
            _metadataFactory = metadataFactory;
            _filter = filter;
        }

        Task ISendObserver.PreSend<T>(SendContext<T> context)
        {
            context.SentTime = DateTime.UtcNow;
            return TaskUtil.Completed;
        }

        Task ISendObserver.PostSend<T>(SendContext<T> context)
        {
            if (!_filter.Matches(context))
                return TaskUtil.Completed;

            var metadata = _metadataFactory.CreateAuditMetadata(context);

            return _store.StoreMessage(context.Message, metadata);
        }

        Task ISendObserver.SendFault<T>(SendContext<T> context, Exception exception) => TaskUtil.Completed;
    }
}
