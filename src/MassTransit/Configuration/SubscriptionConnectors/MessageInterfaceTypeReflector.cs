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
namespace MassTransit.SubscriptionConnectors
{
    using System.Collections.Generic;
    using System.Linq;


    /// <summary>
    ///     Helper class for providing the message reflection for consumers.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class MessageInterfaceTypeReflector<T>
        where T : class
    {
        internal static IEnumerable<MessageInterfaceType> GetAllTypes()
        {
            return GetConsumesContextTypes()
                .Concat(GetConsumesAllTypes());
        }

        internal static IEnumerable<MessageInterfaceType> GetConsumesContextTypes()
        {
            return typeof(T).GetInterfaces()
                .Where(x => x.IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == typeof(Consumes<>.All))
                .Select(x => new MessageInterfaceType(x, x.GetGenericArguments()[0]))
                .Where(x => x.MessageType.IsGenericType)
                .Where(x => x.MessageType.GetGenericTypeDefinition() == typeof(IConsumeContext<>))
                .Select(x => new MessageInterfaceType(x.InterfaceType, x.MessageType.GetGenericArguments()[0]))
                .Where(x => x.MessageType.IsValueType == false);
        }

        internal static IEnumerable<MessageInterfaceType> GetConsumesAllTypes()
        {
            return typeof(T).GetInterfaces()
                .Where(x => x.IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == typeof(Consumes<>.All))
                .Select(x => new MessageInterfaceType(x, x.GetGenericArguments()[0]))
                .Where(x => x.MessageType.IsValueType == false)
                .Where(IsNotContextType);
        }

        internal static IEnumerable<CorrelatedMessageInterfaceType> GetConsumesCorrelatedTypes()
        {
            return typeof(T).GetInterfaces()
                .Where(x => x.IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == typeof(Consumes<>.For<>))
                .Select(
                    x => new CorrelatedMessageInterfaceType(x, x.GetGenericArguments()[0], x.GetGenericArguments()[1]))
                .Where(x => x.MessageType.IsValueType == false)
                .Where(IsNotContextType);
        }

        static bool IsNotContextType(MessageInterfaceType x)
        {
            return !(x.MessageType.IsGenericType &&
                     x.MessageType.GetGenericTypeDefinition() == typeof(IConsumeContext<>));
        }
    }
}