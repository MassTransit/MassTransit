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
namespace MassTransit.RabbitMqTransport.Configuration
{
    using System;


    public interface IRabbitMqServiceBusFactoryConfigurator :
        IServiceBusFactoryConfigurator
    {
        void Host(RabbitMqHostSettings settings);

        /// <summary>
        ///     Specifies that any message sent to an exchange must be delivered to a queue or it
        ///     will be returned to the publisher with an exception
        /// </summary>
        /// <param name="mandatory">True if the message is mandatory, otherwise false</param>
        void Mandatory(bool mandatory = true);

        /// <summary>
        ///     Intercept the publishing of a message type that is assignable to T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="callback"></param>
        void OnPublish<T>(Action<RabbitMqPublishContext<T>> callback)
            where T : class;

        void OnPublish(Action<RabbitMqPublishContext> callback);
    }
}