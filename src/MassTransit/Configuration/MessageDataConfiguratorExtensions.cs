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
namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using GreenPipes;
    using Internals.Extensions;
    using MessageData;
    using Metadata;
    using Transformation.TransformConfigurators;
    using Util;


    public static class MessageDataConfiguratorExtensions
    {
        /// <summary>
        /// Enable the loading of message data for the specified message type. Message data is large data that is
        /// stored outside of the messaging system.
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="configurator"></param>
        /// <param name="repository">The message data repository</param>
        public static void UseMessageData<T>(this IConsumePipeConfigurator configurator, IMessageDataRepository repository)
            where T : class
        {
            var transformConfigurator = new ConsumeTransformSpecification<T>();

            transformConfigurator.LoadMessageData(repository);

            configurator.AddPipeSpecification(transformConfigurator);
        }

        /// <summary>
        /// Enable the loading of message data for the specified message type. Message data is large data that is
        /// stored outside of the messaging system.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="repository"></param>
        /// <param name="configureCallback"></param>
        public static void UseMessageData<T>(this IConsumePipeConfigurator configurator, IMessageDataRepository repository,
            Action<ITransformConfigurator<T>> configureCallback)
            where T : class
        {
            var transformConfigurator = new ConsumeTransformSpecification<T>();

            transformConfigurator.LoadMessageData(repository);

            configureCallback(transformConfigurator);

            configurator.AddPipeSpecification(transformConfigurator);
        }

        /// <summary>
        /// Enable the loading of message data for the specified message type. Message data is large data that is
        /// stored outside of the messaging system.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="repository"></param>
        public static void UseMessageData<T>(this IPipeConfigurator<ConsumeContext<T>> configurator, IMessageDataRepository repository)
            where T : class
        {
            var transformConfigurator = new ConsumeTransformSpecification<T>();

            transformConfigurator.LoadMessageData(repository);

            configurator.AddPipeSpecification(transformConfigurator);
        }

        /// <summary>
        /// Enable the loading of message data for the specified message type. Message data is large data that is
        /// stored outside of the messaging system.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="repository"></param>
        /// <param name="configureCallback"></param>
        public static void UseMessageData<T>(this IPipeConfigurator<ConsumeContext<T>> configurator, IMessageDataRepository repository,
            Action<ITransformConfigurator<T>> configureCallback)
            where T : class
        {
            var transformConfigurator = new ConsumeTransformSpecification<T>();

            transformConfigurator.LoadMessageData(repository);

            configureCallback(transformConfigurator);

            configurator.AddPipeSpecification(transformConfigurator);
        }

        /// <summary>
        /// Load the message data as part of the transform (replaces the property on the original message, to avoid multiple
        /// loads of the same data).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="propertyExpression"></param>
        /// <param name="repository"></param>
        public static void LoadMessageData<T, TProperty>(this ITransformConfigurator<T> configurator, Expression<Func<T, TProperty>> propertyExpression,
            IMessageDataRepository repository)
        {
            var configuration = new LoadMessageDataTransformConfiguration<T, TProperty>(repository, propertyExpression.GetPropertyInfo());

            configuration.Apply(configurator);
        }

        static void LoadMessageData<T>(this ITransformConfigurator<T> configurator, IMessageDataRepository repository)
        {
            List<PropertyInfo> messageDataProperties = TypeMetadataCache<T>.Properties.Where(x => x.PropertyType.HasInterface<IMessageData>()).ToList();
            foreach (PropertyInfo messageDataProperty in messageDataProperties)
            {
                Type dataType = messageDataProperty.PropertyType.GetClosingArguments(typeof(MessageData<>)).First();
                Type providerType = typeof(LoadMessageDataTransformConfiguration<,>).MakeGenericType(typeof(T), dataType);
                var configuration = (IMessageDataTransformConfiguration<T>)Activator.CreateInstance(providerType, repository, messageDataProperty);

                configuration.Apply(configurator);
            }
        }
    }
}