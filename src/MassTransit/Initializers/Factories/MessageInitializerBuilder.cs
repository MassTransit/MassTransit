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
namespace MassTransit.Initializers.Factories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Util;


    public class MessageInitializerBuilder<TMessage, TInput> :
        IMessageInitializerBuilder<TMessage, TInput>
        where TMessage : class
        where TInput : class
    {
        readonly IDictionary<string, IPropertyInitializer<TMessage, TInput>> _initializers;
        readonly IList<IHeaderInitializer<TMessage, TInput>> _headerInitializers;

        public MessageInitializerBuilder()
        {
            if (!TypeMetadataCache<TMessage>.IsValidMessageType)
                throw new ArgumentException(TypeMetadataCache<TMessage>.InvalidMessageTypeReason, nameof(TMessage));

            _initializers = new Dictionary<string, IPropertyInitializer<TMessage, TInput>>(StringComparer.OrdinalIgnoreCase);
            _headerInitializers = new List<IHeaderInitializer<TMessage, TInput>>();
        }

        public IMessageInitializer<TMessage> Build()
        {
            IMessageFactory<TMessage> messageFactory = MessageFactoryCache<TMessage>.Factory;

            return new MessageInitializer<TMessage, TInput>(messageFactory, _initializers.Values, _headerInitializers);
        }

        public void Add(string propertyName, IPropertyInitializer<TMessage> initializer)
        {
            _initializers[propertyName] = new PropertyAdapter(initializer);
        }

        public void Add(string propertyName, IPropertyInitializer<TMessage, TInput> initializer)
        {
            _initializers[propertyName] = initializer;
        }

        public void Add(IHeaderInitializer<TMessage> initializer)
        {
            _headerInitializers.Add(new HeaderAdapter(initializer));
        }

        public void Add(IHeaderInitializer<TMessage, TInput> initializer)
        {
            _headerInitializers.Add(initializer);
        }


        class PropertyAdapter :
            IPropertyInitializer<TMessage, TInput>
        {
            readonly IPropertyInitializer<TMessage> _initializer;

            public PropertyAdapter(IPropertyInitializer<TMessage> initializer)
            {
                _initializer = initializer;
            }

            public Task Apply(InitializeContext<TMessage, TInput> context)
            {
                return _initializer.Apply(context);
            }
        }


        class HeaderAdapter :
            IHeaderInitializer<TMessage, TInput>
        {
            readonly IHeaderInitializer<TMessage> _initializer;

            public HeaderAdapter(IHeaderInitializer<TMessage> initializer)
            {
                _initializer = initializer;
            }

            public Task Apply(InitializeContext<TMessage, TInput> context, SendContext sendContext)
            {
                return _initializer.Apply(context, sendContext);
            }
        }
    }
}