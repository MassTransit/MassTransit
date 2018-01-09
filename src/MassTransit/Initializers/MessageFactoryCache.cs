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
    using GreenPipes.Internals.Extensions;
    using Util;


    public class MessageFactoryCache<TMessage>
        where TMessage : class
    {
        public static IMessageFactory<TMessage> Factory => Cached.MessageFactory.Value;


        static class Cached
        {
            internal static readonly Lazy<IMessageFactory<TMessage>> MessageFactory = new Lazy<IMessageFactory<TMessage>>(() => CreateMessageFactory());

            static IMessageFactory<TMessage> CreateMessageFactory()
            {
                if (!TypeMetadataCache<TMessage>.IsValidMessageType)
                    throw new ArgumentException(TypeMetadataCache<TMessage>.InvalidMessageTypeReason, nameof(TMessage));

                Type implementationType = typeof(TMessage);
                if (typeof(TMessage).IsInterface)
                {
                    implementationType = TypeCache.GetImplementationType(typeof(TMessage));
                }

                return (IMessageFactory<TMessage>)Activator.CreateInstance(typeof(DynamicMessageFactory<,>).MakeGenericType(typeof(TMessage),
                    implementationType));
            }
        }
    }
}