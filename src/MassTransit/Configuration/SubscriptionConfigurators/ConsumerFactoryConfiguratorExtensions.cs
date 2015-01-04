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
namespace MassTransit.SubscriptionConfigurators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Configurators;
    using Internals.Extensions;
    using SubscriptionConnectors;
    using Util;


    public static class ConsumerFactoryConfiguratorExtensions
    {
        public static IEnumerable<ValidationResult> ValidateConsumer<TConsumer>(this Configurator configurator)
            where TConsumer : class
        {
            if (!typeof(TConsumer).HasInterface<IConsumer>())
            {
                yield return configurator.Warning("Consumer",
                    string.Format("The consumer class {0} does not implement any IConsumer interfaces",
                        TypeMetadataCache<TConsumer>.ShortName));
            }

            IEnumerable<ValidationResult> warningForMessages = ConsumerMetadataCache<TConsumer>
                .ConsumerTypes.Concat(ConsumerMetadataCache<TConsumer>.MessageConsumerTypes)
                .Distinct(MessageInterfaceType.MessageTypeComparer)
                .Where(x => !(HasProtectedDefaultConstructor(x.MessageType) || HasSinglePublicConstructor(x.MessageType)))
                .Select(x => string.Format("The {0} message should have a public or protected default constructor." +
                    " Without an available constructor, MassTransit will initialize new message instances" +
                    " without calling a constructor, which can lead to unpredictable behavior if the message" +
                    " depends upon logic in the constructor to be executed.", TypeMetadataCache.ShortName(x.MessageType)))
                .Select(message => configurator.Warning("Message", message));

            foreach (ValidationResult message in warningForMessages)
                yield return message;
        }

        public static IEnumerable<ValidationResult> Validate<TConsumer>(this IConsumerFactory<TConsumer> consumerFactory)
            where TConsumer : class
        {
            if (consumerFactory == null)
                yield return ValidationResultExtensions.Failure(null, "UseConsumerFactory", "must not be null");

            foreach (ValidationResult result in ValidateConsumer<TConsumer>(null))
                yield return result;
        }

        static bool HasProtectedDefaultConstructor(Type type)
        {
            return type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
                .Any(constructorInfo => !constructorInfo.GetParameters().Any());
        }

        static bool HasSinglePublicConstructor(Type type)
        {
            return type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                .All(constructorInfo => !constructorInfo.GetParameters().Any())
                && type.GetConstructors().Count() == 1;
        }
    }
}