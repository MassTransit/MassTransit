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
    using System.Linq;
    using System.Threading;
    using Magnum.Extensions;
    using Saga;


    public class TypeMetadataCache<T> :
        ITypeMetadataCache<T>
    {
        readonly Lazy<bool> _hasSagaInterfaces;

        readonly string _shortName;

        TypeMetadataCache()
        {
            _shortName = typeof(T).ToShortTypeName();

            _hasSagaInterfaces = new Lazy<bool>(ScanForSagaInterfaces, LazyThreadSafetyMode.PublicationOnly);
        }

        public static string ShortName
        {
            get { return InstanceCache.Cached.Value.ShortName; }
        }

        public static bool HasSagaInterfaces
        {
            get { return InstanceCache.Cached.Value.HasSagaInterfaces; }
        }

        bool ITypeMetadataCache<T>.HasSagaInterfaces
        {
            get { return _hasSagaInterfaces.Value; }
        }


        string ITypeMetadataCache<T>.ShortName
        {
            get { return _shortName; }
        }

        static bool ScanForSagaInterfaces()
        {
            Type[] interfaces = typeof(T).GetInterfaces();

            if (interfaces.Contains(typeof(ISaga)))
                return true;

            if (interfaces.Any(x => x.GetGenericTypeDefinition() == typeof(InitiatedBy<>))
                || interfaces.Any(x => x.GetGenericTypeDefinition() == typeof(Orchestrates<>))
                || interfaces.Any(x => x.GetGenericTypeDefinition() == typeof(Observes<,>)))
                return true;

            return false;
        }


        static class InstanceCache
        {
            internal static readonly Lazy<ITypeMetadataCache<T>> Cached = new Lazy<ITypeMetadataCache<T>>(
                () => new TypeMetadataCache<T>(), LazyThreadSafetyMode.PublicationOnly);
        }
    }
}