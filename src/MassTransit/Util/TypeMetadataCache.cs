// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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


        static class Cached
        {
            internal static readonly IDictionaryConverterCache DictionaryConverterCache = new DictionaryConverterCache();
            internal static readonly IImplementationBuilder ImplementationBuilder = new DynamicImplementationBuilder();
            internal static readonly ConcurrentDictionary<Type, CachedType> Instance = new ConcurrentDictionary<Type, CachedType>();
            internal static readonly IObjectConverterCache ObjectConverterCache = new DynamicObjectConverterCache(ImplementationBuilder);
        }


        interface CachedType
        {
            string ShortName { get; }
        }


        class CachedType<T> :
            CachedType
        {
            public string ShortName
            {
                get { return TypeMetadataCache<T>.ShortName; }
            }
        }
    }


    public class TypeMetadataCache<T> :
        ITypeMetadataCache<T>
    {
        readonly Lazy<IDictionaryConverter> _dictionaryConverter;
        readonly Lazy<bool> _hasSagaInterfaces;
        readonly Lazy<Type> _implementationType;
        readonly Lazy<IObjectConverter> _objectConverter;

        readonly string _shortName;

        TypeMetadataCache()
        {
            _shortName = typeof(T).GetTypeName();

            _hasSagaInterfaces = new Lazy<bool>(ScanForSagaInterfaces, LazyThreadSafetyMode.PublicationOnly);

            _implementationType = new Lazy<Type>(() => TypeMetadataCache.GetImplementationType(typeof(T)));

            _dictionaryConverter = new Lazy<IDictionaryConverter>(() => TypeMetadataCache.GetDictionaryConverter(typeof(T)));
            _objectConverter = new Lazy<IObjectConverter>(() => TypeMetadataCache.GetObjectConverter(typeof(T)));
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