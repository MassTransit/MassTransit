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
namespace MassTransit.ConsumeConnectors
{
    using System;
    using System.Linq;
    using System.Threading;
    using Util;


    public class ConsumerMetadataCache<T> :
        IConsumerMetadataCache<T>
        where T : class
    {
        readonly IMessageInterfaceType[] _consumerTypes;

        ConsumerMetadataCache()
        {
            _consumerTypes = ConsumerConventionCache.GetConventions<T>()
                .SelectMany(x => x.GetMessageTypes())
                .Distinct((x, y) => x.MessageType == y.MessageType)
                .ToArray();
        }

        public static IMessageInterfaceType[] ConsumerTypes => Cached.Metadata.Value.ConsumerTypes;

        IMessageInterfaceType[] IConsumerMetadataCache<T>.ConsumerTypes => _consumerTypes;


        static class Cached
        {
            internal static readonly Lazy<IConsumerMetadataCache<T>> Metadata = new Lazy<IConsumerMetadataCache<T>>(
                () => new ConsumerMetadataCache<T>(), LazyThreadSafetyMode.PublicationOnly);
        }
    }
}