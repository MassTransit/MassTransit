// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Util
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Internals.Extensions;
    using Internals.Mapping;
    using Internals.Reflection;
    using Saga;


    public static class TypeMetadataCache
    {
        public static IImplementationBuilder ImplementationBuilder
        {
            get { return Cached.ImplementationBuilder; }
        }

        static CachedType GetOrAdd(Type type)
        {
            return Cached.Instance.GetOrAdd(type, _ =>
                (CachedType)Activator.CreateInstance(typeof(CachedType<>).MakeGenericType(type)));
        }

        public static string ShortName(Type type)
        {
            return GetOrAdd(type).ShortName;
        }

        public static IDictionaryConverter GetDictionaryConverter(Type type)
        {
            return Cached.DictionaryConverterCache.GetConverter(type);
        }

        public static IObjectConverter GetObjectConverter(Type type)
        {
            return Cached.ObjectConverterCache.GetConverter(type);
        }

        public static Type GetImplementationType(Type type)
        {
            return Cached.ImplementationBuilder.GetImplementationType(type);
        }

        public static bool IsValidMessageType(Type type)
        {
            return GetOrAdd(type).IsValidMessageType;
        }

        public static Type[] GetMessageTypes(Type type)
        {
            return GetOrAdd(type).MessageTypes;
        }


        static class Cached
        {
            internal static readonly IDictionaryConverterCache DictionaryConverterCache = new DictionaryConverterCache();
            internal static readonly IImplementationBuilder ImplementationBuilder = new DynamicImplementationBuilder();
            internal static readonly ConcurrentDictionary<Type, CachedType> Instance = new ConcurrentDictionary<Type, CachedType>();
            internal static readonly IObjectConverterCache ObjectConverterCache = new DynamicObjectConverterCache(ImplementationBuilder);
        }


        interface CachedType
        {
            bool IsValidMessageType { get; }
            string ShortName { get; }
            Type[] MessageTypes { get; }
        }


        class CachedType<T> :
            CachedType
        {
            public bool IsValidMessageType
            {
                get { return TypeMetadataCache<T>.IsValidMessageType; }
            }

            public string ShortName
            {
                get { return TypeMetadataCache<T>.ShortName; }
            }

            public Type[] MessageTypes
            {
                get { return TypeMetadataCache<T>.MessageTypes; }
            }
        }
    }


    public class TypeMetadataCache<T> :
        ITypeMetadataCache<T>
    {
        readonly Lazy<IDictionaryConverter> _dictionaryConverter;
        readonly Lazy<bool> _hasSagaInterfaces;
        readonly Lazy<Type> _implementationType;
        readonly Lazy<bool> _isValidMessageType;
        readonly Lazy<Type[]> _messageTypes;
        readonly Lazy<IObjectConverter> _objectConverter;
        readonly Lazy<ReadOnlyPropertyCache<T>> _propertyCache;
        readonly string _shortName;

        TypeMetadataCache()
        {
            _shortName = typeof(T).GetTypeName();

            _hasSagaInterfaces = new Lazy<bool>(ScanForSagaInterfaces, LazyThreadSafetyMode.PublicationOnly);

            _implementationType = new Lazy<Type>(() => TypeMetadataCache.GetImplementationType(typeof(T)));

            _dictionaryConverter = new Lazy<IDictionaryConverter>(() => TypeMetadataCache.GetDictionaryConverter(typeof(T)));
            _objectConverter = new Lazy<IObjectConverter>(() => TypeMetadataCache.GetObjectConverter(typeof(T)));

            _propertyCache = new Lazy<ReadOnlyPropertyCache<T>>(() => new ReadOnlyPropertyCache<T>());

            _isValidMessageType = new Lazy<bool>(CheckIfValidMessageType);
            _messageTypes = new Lazy<Type[]>(() => GetMessageTypes().ToArray());
        }

        public static string ShortName
        {
            get { return Cached.Metadata.Value.ShortName; }
        }

        public static bool HasSagaInterfaces
        {
            get { return Cached.Metadata.Value.HasSagaInterfaces; }
        }

        public static IDictionaryConverter DictionaryConverter
        {
            get { return Cached.Metadata.Value.DictionaryConverter; }
        }

        public static IObjectConverter ObjectConverter
        {
            get { return Cached.Metadata.Value.ObjectConverter; }
        }

        public static ReadOnlyPropertyCache<T> ReadOnlyPropertyCache
        {
            get { return Cached.Metadata.Value.ReadOnlyPropertyCache; }
        }

        public static bool IsValidMessageType
        {
            get { return Cached.Metadata.Value.IsValidMessageType; }
        }

        public static Type[] MessageTypes
        {
            get { return Cached.Metadata.Value.MessageTypes; }
        }

        bool ITypeMetadataCache<T>.IsValidMessageType
        {
            get { return _isValidMessageType.Value; }
        }

        Type[] ITypeMetadataCache<T>.MessageTypes
        {
            get { return _messageTypes.Value; }
        }

        ReadOnlyPropertyCache<T> ITypeMetadataCache<T>.ReadOnlyPropertyCache
        {
            get { return _propertyCache.Value; }
        }

        T ITypeMetadataCache<T>.InitializeFromObject(object values)
        {
            if (values == null)
                throw new ArgumentNullException("values");

            IDictionary<string, object> dictionary = TypeMetadataCache.GetDictionaryConverter(values.GetType())
                .GetDictionary(values);

            return (T)_objectConverter.Value.GetObject(dictionary);
        }

        IDictionaryConverter ITypeMetadataCache<T>.DictionaryConverter
        {
            get { return _dictionaryConverter.Value; }
        }

        IObjectConverter ITypeMetadataCache<T>.ObjectConverter
        {
            get { return _objectConverter.Value; }
        }

        bool ITypeMetadataCache<T>.HasSagaInterfaces
        {
            get { return _hasSagaInterfaces.Value; }
        }

        string ITypeMetadataCache<T>.ShortName
        {
            get { return _shortName; }
        }

        /// <summary>
        /// Returns true if the specified type is an allowed message type, i.e.
        /// that it doesn't come from the .Net core assemblies or is without a namespace,
        /// amongst others.
        /// </summary>
        /// <returns>True if the message can be sent, otherwise false</returns>
        bool CheckIfValidMessageType()
        {
            if (typeof(T).Namespace == null)
                return false;

            if (typeof(T).Assembly == typeof(object).Assembly)
                return false;

            if (typeof(T).Namespace == "System")
                return false;

            string ns = typeof(T).Namespace;
            if (ns != null && ns.StartsWith("System."))
                return false;

            if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(CorrelatedBy<>))
                return false;

            return true;
        }

        /// <summary>
        /// Returns all the message types that are available for the specified type. This will
        /// return any base classes or interfaces implemented by the type that are allowed
        /// message types.
        /// </summary>
        /// <returns>An enumeration of valid message types implemented by the specified type</returns>
        static IEnumerable<Type> GetMessageTypes()
        {
            if (IsValidMessageType)
                yield return typeof(T);

            Type baseType = typeof(T).BaseType;
            while ((baseType != null) && TypeMetadataCache.IsValidMessageType(baseType))
            {
                yield return baseType;

                baseType = baseType.BaseType;
            }

            IEnumerable<Type> interfaces = typeof(T)
                .GetInterfaces()
                .Where(TypeMetadataCache.IsValidMessageType);

            foreach (Type interfaceType in interfaces)
                yield return interfaceType;
        }

        public static T InitializeFromObject(object values)
        {
            return Cached.Metadata.Value.InitializeFromObject(values);
        }

        static bool ScanForSagaInterfaces()
        {
            Type[] interfaces = typeof(T).GetInterfaces();

            if (interfaces.Contains(typeof(ISaga)))
                return true;

            return interfaces.Any(x => x.HasInterface(typeof(InitiatedBy<>))
                || x.HasInterface(typeof(Orchestrates<>))
                || x.HasInterface(typeof(Observes<,>)));
        }


        static class Cached
        {
            internal static readonly Lazy<ITypeMetadataCache<T>> Metadata = new Lazy<ITypeMetadataCache<T>>(
                () => new TypeMetadataCache<T>(), LazyThreadSafetyMode.PublicationOnly);
        }
    }
}