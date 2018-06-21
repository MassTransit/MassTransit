// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Collections.Generic;
    using Context;


    public interface IObjectTypeDeserializer
    {
        T Deserialize<T>(IDictionary<string, object> dictionary, string key, T defaultValue);
        T Deserialize<T>(IHeaderProvider headerProvider, string key, T defaultValue);

        T Deserialize<T>(IDictionary<string, object> dictionary, string key);

        T Deserialize<T>(IDictionary<string,object> dictionary);
        T Deserialize<T>(object value);
        T Deserialize<T>(object value, T defaultValue);
        object Deserialize(object value, Type objectType, bool allowNull = false);
    }
}