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
    using System;
    using System.Threading.Tasks;
    using MassTransit;


    public class MessageAuditConsumeObserver : IConsumeObserver
    {
        public delegate MessageAuditMetadata MetadataFactory(ConsumeContext context);

        readonly MessageAuditStore _store;
        readonly MetadataFactory _metaDataFactory;
        readonly AuditSpecification _specification;

        public MessageAuditConsumeObserver(MessageAuditStore store,
            MetadataFactory metaDataFactory,
            Action<IMessageFilterConfigurator> configure)
        {
            _store = store;
            _metaDataFactory = metaDataFactory;
            _specification = new AuditSpecification();
            configure?.Invoke(_specification);
        }

        public async Task PreConsume<T>(ConsumeContext<T> context) where T : class
        {
            if (!_specification.Filter(context.Message))
                return;

            await _store.StoreMessage(
                context.Message,
                context.Message.GetType().FullName,
                _metaDataFactory(context));
        }

        public Task PostConsume<T>(ConsumeContext<T> context) where T : class
            => Task.FromResult(0);

        public Task ConsumeFault<T>(ConsumeContext<T> context, Exception exception) where T : class
            => Task.FromResult(0);
    }
}