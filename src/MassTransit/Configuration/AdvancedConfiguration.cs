// Copyright 2007-2010 The Apache Software Foundation.
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
namespace MassTransit.Configuration
{
    using System;

    public interface AdvancedConfiguration
    {
        /// <summary>
        /// Specifies a configuration action to perform on a particular endpoint when it is created.
        /// </summary>
        /// <param name="uriString">The Uri to be configured (matching is case insensitive)</param>
        /// <param name="action">The action to perform when the transport for the endpoint is created.</param>
        void ConfigureEndpoint(string uriString, Action<IEndpointConfigurator> action);

        /// <summary>
        /// Specifies a configuration action to perform on a particular endpoint when it is created.
        /// </summary>
        /// <param name="uri">The Uri to be configured</param>
        /// <param name="action">The action to perform when the transport for the endpoint is created.</param>
        void ConfigureEndpoint(Uri uri, Action<IEndpointConfigurator> action);


        void SetConcurrentConsumerLimit(int concurrentConsumerLimit);
        void SetConcurrentReceiverLimit(int concurrentReceiverLimit);

        /// <summary>
        /// Specifies an action to call before a message is consumed
        /// </summary>
        /// <param name="beforeConsume"></param>
        void BeforeConsumingMessage(Action beforeConsume);

        /// <summary>
        /// Specifies an action to call after a message is consumed
        /// </summary>
        /// <param name="afterConsume"></param>
        void AfterConsumingMessage(Action afterConsume);
    }
}