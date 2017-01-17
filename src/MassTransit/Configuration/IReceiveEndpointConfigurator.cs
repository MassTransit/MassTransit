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
    using System;
    using System.ComponentModel;


    /// <summary>
    /// Configure a receiving endpoint
    /// </summary>
    public interface IReceiveEndpointConfigurator :
        IConsumePipeConfigurator,
        ISendPipelineConfigurator,
        IPublishPipelineConfigurator
    {
        /// <summary>
        /// Returns the input address of the receive endpoint
        /// </summary>
        Uri InputAddress { get; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        void AddEndpointSpecification(IReceiveEndpointSpecification configurator);
    }
}