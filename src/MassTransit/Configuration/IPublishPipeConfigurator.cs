// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.ComponentModel;
    using GreenPipes;
    using PublishPipeSpecifications;


    public interface IPublishPipeConfigurator :
        IPipeConfigurator<PublishContext>,
        IPublishPipeSpecificationObserverConnector
    {
        /// <summary>
        /// Adds a type-specific pipe specification to the consume pipe
        /// </summary>
        /// <param name="specification"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        void AddPipeSpecification(IPipeSpecification<SendContext> specification);

        /// <summary>
        /// Adds a type-specific pipe specification to the consume pipe
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="specification"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        void AddPipeSpecification<T>(IPipeSpecification<SendContext<T>> specification)
            where T : class;

        /// <summary>
        /// Adds a type-specific pipe specification to the consume pipe
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="specification"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        void AddPipeSpecification<T>(IPipeSpecification<PublishContext<T>> specification)
            where T : class;
    }
}