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
namespace MassTransit.Serialization
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using Newtonsoft.Json;


    public static class ListConverterCache
    {
        static ListConverter GetOrAdd(Type type)
        {
            return InstanceCache.Cached.GetOrAdd(type, _ =>
                (ListConverter)Activator.CreateInstance(typeof(CachedConverter<>).MakeGenericType(type)));
        }

        public static object GetList(Type elementType, JsonReader reader, JsonSerializer serializer, bool toArray)
        {
            return GetOrAdd(elementType).GetList(reader, serializer, toArray);
        }


        class CachedConverter<T> :
            ListConverter
        {
            public object GetList(JsonReader reader, JsonSerializer serializer, bool toArray)
            {
//                if (reader.TokenType == JsonToken.Null)
//                    return null;

                var list = new List<T>();

                if (reader.TokenType == JsonToken.StartArray)
                    serializer.Populate(reader, list);
                else
                {
                    var item = (T)serializer.Deserialize(reader, typeof(T));
                    list.Add(item);
                }

                if (toArray)
                    return list.ToArray();

                return list;
            }
        }


        static class InstanceCache
        {
            internal static readonly ConcurrentDictionary<Type, ListConverter> Cached =
                new ConcurrentDictionary<Type, ListConverter>();
        }


        interface ListConverter
        {
            object GetList(JsonReader reader, JsonSerializer serializer, bool toArray);
        }
    }
}