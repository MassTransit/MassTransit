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
namespace MassTransit.Serialization.JsonConverters
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Internals.Extensions;
    using Newtonsoft.Json;


    public class ListJsonConverter :
        JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotSupportedException("This converter should not be used for writing as it can create loops");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            if (objectType.IsArray)
            {
                Type elementType = objectType.GetElementType();

                return ListConverterCache.GetList(elementType, reader, serializer, true);
            }
            else
            {
                Type elementType = objectType.GetGenericArguments()[0];
                return ListConverterCache.GetList(elementType, reader, serializer, false);
            }
        }

        public override bool CanConvert(Type objectType)
        {
            if (objectType.IsGenericType
                && (objectType.GetGenericTypeDefinition() == typeof(IList<>) || objectType.GetGenericTypeDefinition() == typeof(List<>)))
                return true;

            if (objectType.IsArray)
            {
                if (objectType.HasElementType && objectType.GetElementType() == typeof(byte))
                    return false;

                return objectType.HasInterface<IEnumerable>();
            }

            return false;
        }
    }
}