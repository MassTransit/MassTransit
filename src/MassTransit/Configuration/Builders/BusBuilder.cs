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
namespace MassTransit.Builders
{
    using System;
    using Configuration;


    /// <summary>
    /// A BusBuilder is the base interface for building service, and includes things like
    /// control bus, subscription bus, and other limited-functionality bus instances
    /// </summary>
    public interface BusBuilder
    {
        /// <summary>
        /// The settings to creating the bus, including address, etc.
        /// </summary>
        BusSettings Settings { get; }

        /// <summary>
        /// Builds the bus instance
        /// </summary>
        /// <returns></returns>
        IServiceBus Build();

        /// <summary>
        /// Adds an action to be performed after bus creation to adjust settings, etc.
        /// but before the bus is started.
        /// </summary>
        /// <param name="postCreateAction"></param>
        void AddPostCreateAction(Action<ServiceBus> postCreateAction);
    }
}