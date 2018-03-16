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
namespace MassTransit.Initializers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Conventions;
    using GreenPipes.Internals.Extensions;


    public class MessageInitializerCache<TMessage> :
        IMessageInitializerCache<TMessage>
        where TMessage : class
    {
        readonly IDictionary<Type, Lazy<IMessageInitializer<TMessage>>> _initializers;

        MessageInitializerCache()
        {
            _initializers = new Dictionary<Type, Lazy<IMessageInitializer<TMessage>>>();
        }

        IMessageInitializer<TMessage, TInput> IMessageInitializerCache<TMessage>.GetInitializer<TInput>()
        {
            Lazy<IMessageInitializer<TMessage>> result;
            lock (_initializers)
            {
                if (_initializers.TryGetValue(typeof(TInput), out Lazy<IMessageInitializer<TMessage>> initializer))
                    return initializer.Value as IMessageInitializer<TMessage, TInput>;

                result = new Lazy<IMessageInitializer<TMessage>>(CreateMessageInitializer<TInput>);

                _initializers[typeof(TInput)] = result;
            }

            return result.Value as IMessageInitializer<TMessage, TInput>;
        }

        static IMessageInitializer<TMessage, TInput> CreateMessageInitializer<TInput>()
            where TInput : class
        {
            var builder = new MessageInitializerBuilder<TMessage, TInput>();

            var inspectors = CreatePropertyInspectors<TInput>().ToArray();

            var convention = new CopyMessageInitializerConvention();
            foreach (var inspector in inspectors)
                inspector.Apply(builder, convention);

            IMessageInitializer<TMessage, TInput> messageInitializer = builder.Build();

            return messageInitializer;
        }

        static IEnumerable<IMessagePropertyInitializerInspector<TMessage, TInput>> CreatePropertyInspectors<TInput>()
            where TInput : class
        {
            return typeof(TMessage).GetAllProperties().Where(x => x.CanRead)
                .Select(x => (IMessagePropertyInitializerInspector<TMessage, TInput>)Activator.CreateInstance(
                    typeof(MessagePropertyInitializerInspector<,,>).MakeGenericType(typeof(TMessage), typeof(TInput), x.PropertyType), x.Name));
        }

        /// <summary>
        /// Returns the initializer for the message/input type combination
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <returns></returns>
        public static IMessageInitializer<TMessage, TInput> GetInitializer<TInput>()
            where TInput : class
        {
            return Cached.InitializerCache.GetInitializer<TInput>();
        }

        public static Task<TMessage> Initialize<T>(T input, CancellationToken cancellationToken = default)
            where T : class
        {
            return Cached.InitializerCache.GetInitializer<T>().Initialize(input, cancellationToken);
        }


        static class Cached
        {
            internal static readonly IMessageInitializerCache<TMessage> InitializerCache = new MessageInitializerCache<TMessage>();
        }
    }
}