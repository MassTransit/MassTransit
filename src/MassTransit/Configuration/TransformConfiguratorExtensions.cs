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
    using TransformConfigurators;


    public static class TransformConfiguratorExtensions
    {
        /// <summary>
        /// Encapsulate the pipe behavior in a transaction
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="configureCallback"></param>
        public static void Transform<T>(this IConsumePipeConfigurator configurator, Action<ITransformConfigurator<T>> configureCallback)
            where T : class
        {
            var transformConfigurator = new TransformConsumePipeSpecification<T>();

            configureCallback(transformConfigurator);

            configurator.AddPipeSpecification(transformConfigurator);
        }

        /// <summary>
        /// Encapsulate the pipe behavior in a transaction
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="configureTransform"></param>
        public static void UseTransform<T>(this IConsumePipeConfigurator configurator,
            Func<ITransformSpecificationConfigurator<T>, ITransformConfiguration<T>> configureTransform)
            where T : class
        {
            var specificationConfigurator = new TransformSpecificationConfigurator<T>();

            ITransformConfiguration<T> specification = configureTransform(specificationConfigurator);

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Encapsulate the pipe behavior in a transaction
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="configureCallback"></param>
        public static void Transform<T>(this IPipeConfigurator<ConsumeContext<T>> configurator, Action<ITransformConfigurator<T>> configureCallback)
            where T : class
        {
            var transformConfigurator = new TransformConsumePipeSpecification<T>();

            configureCallback(transformConfigurator);

            configurator.AddPipeSpecification(transformConfigurator);
        }

        /// <summary>
        /// Encapsulate the pipe behavior in a transaction
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="configureTransform"></param>
        public static void UseTransform<T>(this IPipeConfigurator<ConsumeContext<T>> configurator,
            Func<ITransformSpecificationConfigurator<T>, ITransformConfiguration<T>> configureTransform)
            where T : class
        {
            var specificationConfigurator = new TransformSpecificationConfigurator<T>();

            ITransformConfiguration<T> specification = configureTransform(specificationConfigurator);

            configurator.AddPipeSpecification(specification);
        }
    }
}