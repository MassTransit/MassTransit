// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Collections.Generic;
    using Magnum.Reflection;
    using Newtonsoft.Json;

//    public class DictionaryJsonConverter :
//        JsonConverter
//    {
//        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
//        {
//            throw new NotSupportedException("This converter should not be used for writing as it can create loops");
//        }
//
//        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
//                                        JsonSerializer serializer)
//        {
//            Type[] arguments = objectType.GetGenericArguments();
//
//            Type keyType = arguments[0];
//            Type valueType = arguments[1];
//            return this.FastInvoke<DictionaryJsonConverter, object>(new[] {keyType, valueType}, "GetDictionary", reader,
//                objectType, serializer);
//        }
//
//        public override bool CanConvert(Type objectType)
//        {
//            return (objectType.IsGenericType
//                    && (objectType.GetGenericTypeDefinition() == typeof (IDictionary<,>)
//                        || objectType.GetGenericTypeDefinition() == typeof (Dictionary<,>)));
//        }
//
//        object GetDictionary<TKey, TValue>(JsonReader reader, Type objectType, JsonSerializer serializer)
//        {
//            var dictionary = new Dictionary<TKey, TValue>();
//
//            if (reader.TokenType == JsonToken.Null)
//                return dictionary;
//
//            serializer.Populate(reader, dictionary);
//
//            return dictionary;
//        }
//    }
}