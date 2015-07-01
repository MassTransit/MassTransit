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
namespace MassTransit.Transformation.TransformConfigurators
{
    using System;


    public interface ITransformSpecificationConfigurator<TMessage>
        where TMessage : class
    {
        /// <summary>
        /// Get a transform specification using the default constructor
        /// </summary>
        /// <typeparam name="T">The transform specification type</typeparam>
        /// <returns></returns>
        IConsumeTransformSpecification<TMessage> Get<T>()
            where T : IConsumeTransformSpecification<TMessage>, new();

        /// <summary>
        /// Get a transform specification using the factory method
        /// </summary>
        /// <typeparam name="T">The transform specification type</typeparam>
        /// <param name="transformFactory">The transform specification factory method</param>
        /// <returns></returns>
        IConsumeTransformSpecification<TMessage> Get<T>(Func<T> transformFactory)
            where T : IConsumeTransformSpecification<TMessage>;
    }
}