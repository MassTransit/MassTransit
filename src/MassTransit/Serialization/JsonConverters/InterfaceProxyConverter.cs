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
    using System.Reflection;
    using GreenPipes.Internals.Reflection;
    using Newtonsoft.Json;
    using Util;


    public class InterfaceProxyConverter :
        JsonConverter
    {
        readonly IImplementationBuilder _builder;

        public InterfaceProxyConverter(IImplementationBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            
            _builder = builder;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Type proxyType = _builder.GetImplementationType(objectType);

            return serializer.Deserialize(reader, proxyType);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.GetTypeInfo().IsInterface && TypeMetadataCache.IsValidMessageType(objectType);
        }
    }
}