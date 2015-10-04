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
namespace MassTransit.Courier
{
    using System;
    using Contracts;
    using Util;


    public static class RoutingSlipEventExtensions
    {
        public static T GetResult<T>(this RoutingSlipActivityCompensated source, string key)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The key must not be empty", nameof(key));

            return ObjectTypeDeserializer.Deserialize<T>(source.Data, key);
        }

        public static T GetVariable<T>(this RoutingSlipActivityCompensated source, string key)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The key must not be empty", nameof(key));

            return ObjectTypeDeserializer.Deserialize<T>(source.Variables, key);
        }

        public static T GetResult<T>(this RoutingSlipActivityCompensationFailed source, string key)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The key must not be empty", nameof(key));

            return ObjectTypeDeserializer.Deserialize<T>(source.Data, key);
        }

        public static T GetVariable<T>(this RoutingSlipActivityCompensationFailed source, string key)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The key must not be empty", nameof(key));

            return ObjectTypeDeserializer.Deserialize<T>(source.Variables, key);
        }

        public static T GetArgument<T>(this RoutingSlipActivityCompleted source, string key)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The key must not be empty", nameof(key));

            return ObjectTypeDeserializer.Deserialize<T>(source.Arguments, key);
        }

        public static T GetResult<T>(this RoutingSlipActivityCompleted source, string key)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The key must not be empty", nameof(key));

            return ObjectTypeDeserializer.Deserialize<T>(source.Data, key);
        }

        public static T GetVariable<T>(this RoutingSlipActivityCompleted source, string key)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The key must not be empty", nameof(key));

            return ObjectTypeDeserializer.Deserialize<T>(source.Variables, key);
        }

        public static T GetArgument<T>(this RoutingSlipActivityFaulted source, string key)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The key must not be empty", nameof(key));

            return ObjectTypeDeserializer.Deserialize<T>(source.Arguments, key);
        }

        public static T GetVariable<T>(this RoutingSlipActivityFaulted source, string key)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The key must not be empty", nameof(key));

            return ObjectTypeDeserializer.Deserialize<T>(source.Variables, key);
        }

        public static T GetVariable<T>(this RoutingSlipCompensationFailed source, string key)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The key must not be empty", nameof(key));

            return ObjectTypeDeserializer.Deserialize<T>(source.Variables, key);
        }

        public static T GetVariable<T>(this RoutingSlipCompleted source, string key)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The key must not be empty", nameof(key));

            return ObjectTypeDeserializer.Deserialize<T>(source.Variables, key);
        }

        public static T GetVariable<T>(this RoutingSlipFaulted source, string key)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The key must not be empty", nameof(key));

            return ObjectTypeDeserializer.Deserialize<T>(source.Variables, key);
        }
    }
}