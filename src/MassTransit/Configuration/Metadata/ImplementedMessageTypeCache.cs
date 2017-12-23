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
namespace MassTransit.Metadata
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using Util;


    public class ImplementedMessageTypeCache<TMessage> :
        IImplementedMessageTypeCache<TMessage>
        where TMessage : class
    {
        readonly CachedType[] _implementedTypes;

        ImplementedMessageTypeCache()
        {
            _implementedTypes = GetMessageTypes()
                .Where(x => x.Type != typeof(TMessage))
                .Select(x => Activator.CreateInstance(typeof(TypeAdapter<>).MakeGenericType(typeof(TMessage), x.Type), (object)x.Direct))
                .Cast<CachedType>()
                .ToArray();
        }

        void IImplementedMessageTypeCache<TMessage>.EnumerateImplementedTypes(IImplementedMessageType implementedMessageType, bool includeActualType)
        {
            for (var i = 0; i < _implementedTypes.Length; i++)
            {
                if (_implementedTypes[i].MessageType == typeof(TMessage) && !includeActualType)
                    continue;

                _implementedTypes[i].ImplementsType(implementedMessageType);
            }
        }

        /// <summary>
        /// Enumerate the implemented message types
        /// </summary>
        /// <param name="implementedMessageType">The interface reference to invoke for each type</param>
        /// <param name="includeActualType">Include the actual message type first, before any implemented types</param>
        public static void EnumerateImplementedTypes(IImplementedMessageType implementedMessageType, bool includeActualType = false)
        {
            Cached.Instance.Value.EnumerateImplementedTypes(implementedMessageType, includeActualType);
        }

        static IEnumerable<ImplementedType> GetMessageTypes()
        {
            if (TypeMetadataCache<TMessage>.IsValidMessageType)
                yield return new ImplementedType(typeof(TMessage), true);

            var implementedInterfaces = GetImplementedInterfaces(typeof(TMessage)).ToArray();

            foreach (var baseInterface in implementedInterfaces.Except(implementedInterfaces.SelectMany(x => x.GetInterfaces())).Where(TypeMetadataCache.IsValidMessageType))
            {
                yield return new ImplementedType(baseInterface, true);
            }

            foreach (var baseInterface in implementedInterfaces.SelectMany(x => x.GetInterfaces()).Distinct().Where(TypeMetadataCache.IsValidMessageType))
            {
                yield return new ImplementedType(baseInterface, false);
            }
            
            var baseType = typeof(TMessage).GetTypeInfo().BaseType;
            while (baseType != null && TypeMetadataCache.IsValidMessageType(baseType))
            {
                yield return new ImplementedType(baseType, typeof(TMessage).GetTypeInfo().BaseType == baseType);

                foreach (var baseInterface in GetImplementedInterfaces(baseType).Where(TypeMetadataCache.IsValidMessageType))
                {
                    yield return new ImplementedType(baseInterface, false);
                }

                baseType = baseType.GetTypeInfo().BaseType;
            }
        }

        static IEnumerable<Type> GetImplementedInterfaces(Type baseType)
        {
            var baseTypeInfo = baseType.GetTypeInfo();

            IEnumerable<Type> baseInterfaces = baseTypeInfo
                .GetInterfaces()
                .Where(TypeMetadataCache.IsValidMessageType)
                .ToArray();

            if (baseTypeInfo.BaseType != null && baseTypeInfo.BaseType != typeof(object))
                baseInterfaces = baseInterfaces
                    .Except(baseTypeInfo.BaseType.GetInterfaces())
                    .Except(baseInterfaces.SelectMany(x => x.GetInterfaces()))
                    .ToArray();

            return baseInterfaces;
        }


        struct ImplementedType
        {
            /// <summary>
            /// The implemented type
            /// </summary>
            public readonly Type Type;

            /// <summary>
            /// True if the interface is directly implemented by the type
            /// </summary>
            public readonly bool Direct;

            public ImplementedType(Type type, bool direct)
            {
                Type = type;
                Direct = direct;
            }
        }


        interface CachedType
        {
            Type MessageType { get; }
            bool Direct { get; }
            void ImplementsType(IImplementedMessageType implementedMessageType);
        }


        static class Cached
        {
            internal static readonly Lazy<IImplementedMessageTypeCache<TMessage>> Instance = new Lazy<IImplementedMessageTypeCache<TMessage>>(
                () => new ImplementedMessageTypeCache<TMessage>(), LazyThreadSafetyMode.PublicationOnly);
        }


        class TypeAdapter<TAdapter> :
            CachedType
            where TAdapter : class
        {
            public TypeAdapter(bool direct)
            {
                Direct = direct;
            }

            public bool Direct { get; }
            Type CachedType.MessageType => typeof(TAdapter);

            void CachedType.ImplementsType(IImplementedMessageType implementedMessageType)
            {
                implementedMessageType.ImplementsMessageType<TAdapter>(Direct);
            }
        }
    }
}