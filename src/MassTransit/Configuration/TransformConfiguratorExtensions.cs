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
    using GreenPipes;
    using Transformation.TransformConfigurators;


    public static class TransformConfiguratorExtensions
    {
        /// <summary>
        /// Apply a message transform, the behavior of which is defined inline using the configurator
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="configurator">The consume pipe configurator</param>
        /// <param name="configure">The configuration callback</param>
        public static void UseTransform<T>(this IConsumePipeConfigurator configurator, Action<ITransformConfigurator<T>> configure)
            where T : class
        {
            var specification = new ConsumeTransformSpecification<T>();

            configure(specification);

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Encapsulate the pipe behavior in a transaction
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="getSpecification"></param>
        public static void UseTransform<T>(this IConsumePipeConfigurator configurator,
            Func<ITransformSpecificationConfigurator<T>, IConsumeTransformSpecification<T>> getSpecification)
            where T : class
        {
            var specificationConfigurator = new TransformSpecificationConfigurator<T>();

            IConsumeTransformSpecification<T> specification = getSpecification(specificationConfigurator);

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Apply a message transform, the behavior of which is defined inline using the configurator
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="configurator">The consume pipe configurator</param>
        /// <param name="configure">The configuration callback</param>
        public static void UseTransform<T>(this IPipeConfigurator<ConsumeContext<T>> configurator, Action<ITransformConfigurator<T>> configure)
            where T : class
        {
            var specification = new ConsumeTransformSpecification<T>();

            configure(specification);

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Encapsulate the pipe behavior in a transaction
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="getSpecification"></param>
        public static void UseTransform<T>(this IPipeConfigurator<ConsumeContext<T>> configurator,
            Func<ITransformSpecificationConfigurator<T>, IConsumeTransformSpecification<T>> getSpecification)
            where T : class
        {
            var specificationConfigurator = new TransformSpecificationConfigurator<T>();

            IConsumeTransformSpecification<T> specification = getSpecification(specificationConfigurator);

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Apply a transform on send to the message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="configurator">The consume pipe configurator</param>
        /// <param name="configure">The configuration callback</param>
        public static void UseTransform<T>(this ISendPipeConfigurator configurator, Action<ITransformConfigurator<T>> configure)
            where T : class
        {
            var specification = new SendTransformSpecification<T>();

            configure(specification);

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Apply a transform on send to the message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="configurator">The consume pipe configurator</param>
        /// <param name="configure">The configuration callback</param>
        public static void UseTransform<T>(this IPublishPipeConfigurator configurator, Action<ITransformConfigurator<T>> configure)
            where T : class
        {
            var specification = new SendTransformSpecification<T>();

            configure(specification);

            configurator.AddPipeSpecification(specification);
        }
    }
}