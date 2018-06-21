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


    public class AuditConsumeObserver :
        IConsumeObserver
    {
        readonly CompositeFilter<ConsumeContext> _filter;
        readonly IConsumeMetadataFactory _metadataFactory;

        readonly IMessageAuditStore _store;

        public AuditConsumeObserver(IMessageAuditStore store, IConsumeMetadataFactory metadataFactory, CompositeFilter<ConsumeContext> filter)
        {
            _store = store;
            _metadataFactory = metadataFactory;
            _filter = filter;
        }

        Task IConsumeObserver.PreConsume<T>(ConsumeContext<T> context)
        {
            if (!_filter.Matches(context))
                return TaskUtil.Completed;

            var metadata = _metadataFactory.CreateAuditMetadata(context);

            return _store.StoreMessage(context.Message, metadata);
        }

        Task IConsumeObserver.PostConsume<T>(ConsumeContext<T> context) => TaskUtil.Completed;

        Task IConsumeObserver.ConsumeFault<T>(ConsumeContext<T> context, Exception exception) => TaskUtil.Completed;
    }
}