// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using GreenPipes;
    using Microsoft.ServiceBus.Messaging;


    /// <summary>
    /// Handles the connection of a messaging factory
    /// </summary>
    public interface MessagingFactoryContext :
        PipeContext
    {
        MessagingFactory MessagingFactory { get; }

        /// <summary>
        /// The base address of the messaging factory, which will not include any scope within the namespace
        /// </summary>
        Uri ServiceAddress { get; }
    }
}